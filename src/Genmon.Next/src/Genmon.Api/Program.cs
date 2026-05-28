using System.Security.Cryptography;
using System.Text;
using Genmon.Api.Auth;
using Genmon.Application.Dashboard;
using Genmon.Application.Generators;
using Genmon.Application.Localization;
using Genmon.Infrastructure;
using Genmon.Infrastructure.Identity;
using Genmon.Infrastructure.Initialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddSingleton<JwtTokenIssuer>();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services.AddScoped<GeneratorManagementService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddHealthChecks();

var configuredJwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
var jwtSigningKey = configuredJwtOptions.SigningKey;

if (string.IsNullOrWhiteSpace(jwtSigningKey))
{
    if (!builder.Environment.IsDevelopment())
    {
        throw new InvalidOperationException("Jwt:SigningKey must be configured outside development.");
    }

    jwtSigningKey = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
}

var jwtOptions = new JwtOptions
{
    Issuer = configuredJwtOptions.Issuer,
    Audience = configuredJwtOptions.Audience,
    SigningKey = jwtSigningKey,
    ExpiresMinutes = configuredJwtOptions.ExpiresMinutes
};

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services, app.Configuration);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.MapPost("/api/auth/login", async (
    LoginRequest request,
    UserManager<AppUser> userManager,
    JwtTokenIssuer tokenIssuer) =>
{
    var user = await userManager.FindByNameAsync(request.UserName.Trim());
    if (user is null)
    {
        return Results.Unauthorized();
    }

    var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
    if (!validPassword)
    {
        return Results.Unauthorized();
    }

    var roles = await userManager.GetRolesAsync(user);
    return Results.Ok(new LoginResponse(tokenIssuer.Create(user, roles), jwtOptions.ExpiresMinutes * 60, user.PreferredLanguage));
}).AllowAnonymous();

app.MapGet("/api/generators", async (GeneratorManagementService service, CancellationToken cancellationToken)
    => Results.Ok(await service.ListAsync(cancellationToken)))
    .RequireAuthorization();

app.MapGet("/api/generators/{id:guid}", async (Guid id, GeneratorManagementService service, CancellationToken cancellationToken) =>
{
    var generator = await service.GetAsync(id, cancellationToken);
    return generator is null ? Results.NotFound() : Results.Ok(generator);
}).RequireAuthorization();

app.MapPost("/api/generators", async (CreateGeneratorRequest request, GeneratorManagementService service, CancellationToken cancellationToken) =>
{
    var generator = await service.CreateAsync(request, cancellationToken);
    return Results.Created($"/api/generators/{generator.Id}", generator);
}).RequireAuthorization(policy => policy.RequireRole("Administrator", "Operator"));

app.MapPut("/api/generators/{id:guid}", async (Guid id, UpdateGeneratorRequest request, GeneratorManagementService service, CancellationToken cancellationToken) =>
{
    var generator = await service.UpdateAsync(id, request, cancellationToken);
    return generator is null ? Results.NotFound() : Results.Ok(generator);
}).RequireAuthorization(policy => policy.RequireRole("Administrator", "Operator"));

app.MapGet("/api/dashboard", async (DashboardService service, CancellationToken cancellationToken)
    => Results.Ok(await service.GetAsync(cancellationToken)))
    .RequireAuthorization();

app.MapGet("/api/localizations/{culture}", async (string culture, LocalizationService service, CancellationToken cancellationToken)
    => Results.Ok(await service.GetCultureAsync(culture, cancellationToken)))
    .RequireAuthorization();

app.MapGet("/", () => Results.Redirect("/health/live"));

app.Run();

internal sealed record LoginRequest(string UserName, string Password);
internal sealed record LoginResponse(string AccessToken, int ExpiresInSeconds, string PreferredLanguage);
