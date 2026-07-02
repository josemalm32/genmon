using Genmon.Application.Contracts;
using Genmon.Infrastructure.Background;
using Genmon.Infrastructure.Persistence;
using Genmon.Infrastructure.Persistence.Repositories;
using Genmon.Infrastructure.Protocols;
using Genmon.Protocols.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genmon.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        var connectionString = configuration.GetConnectionString("Genmon")
            ?? "Data Source=App_Data/genmon-next.db";
        connectionString = NormalizeSqliteConnectionString(connectionString, contentRootPath);

        services.AddDbContext<GenmonDbContext>(options => options.UseSqlite(connectionString));

        services
            .AddIdentityCore<Identity.AppUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 12;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<Identity.AppRole>()
            .AddEntityFrameworkStores<GenmonDbContext>();

        services.AddScoped<IGeneratorRepository, GeneratorRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<ILocalizationRepository, LocalizationRepository>();

        services.AddSingleton<IGeneratorProtocolAdapter>(new PlaceholderGeneratorProtocolAdapter(Domain.Enums.ControllerType.Unknown));
        services.AddSingleton<IGeneratorProtocolAdapter>(new PlaceholderGeneratorProtocolAdapter(Domain.Enums.ControllerType.GeneracEvolution));
        services.AddSingleton<IGeneratorProtocolAdapter>(new PlaceholderGeneratorProtocolAdapter(Domain.Enums.ControllerType.GeneracNexus));
        services.AddSingleton<IGeneratorProtocolAdapter>(new PlaceholderGeneratorProtocolAdapter(Domain.Enums.ControllerType.HPanel));
        services.AddSingleton<IGeneratorProtocolAdapter>(new PlaceholderGeneratorProtocolAdapter(Domain.Enums.ControllerType.PowerZone));
        services.AddSingleton<IGeneratorProtocolAdapter>(new PlaceholderGeneratorProtocolAdapter(Domain.Enums.ControllerType.Custom));
        services.AddSingleton<IGeneratorProtocolAdapterResolver, GeneratorProtocolAdapterResolver>();

        services.AddHostedService<GeneratorPollingBackgroundService>();
        return services;
    }
    private static string NormalizeSqliteConnectionString(string connectionString, string contentRootPath)
    {
        var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(builder.DataSource) || Path.IsPathRooted(builder.DataSource))
        {
            return builder.ConnectionString;
        }

        var absolutePath = Path.GetFullPath(Path.Combine(contentRootPath, builder.DataSource));
        var directory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        builder.DataSource = absolutePath;
        return builder.ConnectionString;
    }
}
