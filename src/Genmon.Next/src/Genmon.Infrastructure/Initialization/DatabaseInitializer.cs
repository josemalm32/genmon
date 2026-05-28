using Genmon.Domain.Entities;
using Genmon.Infrastructure.Identity;
using Genmon.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genmon.Infrastructure.Initialization;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GenmonDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        foreach (var roleName in new[] { "Administrator", "Operator", "Viewer" })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new AppRole { Name = roleName });
            }
        }

        var adminSection = configuration.GetSection("BootstrapAdmin");
        var bootstrapEnabled = adminSection.GetValue("Enabled", false);
        var adminUserName = adminSection["UserName"] ?? "admin";
        var adminPassword = adminSection["Password"];
        var adminEmail = adminSection["Email"] ?? "admin@genmon.local";
        var adminLanguage = adminSection["PreferredLanguage"] ?? "en";

        if (bootstrapEnabled)
        {
            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException("BootstrapAdmin:Password must be configured when bootstrap admin is enabled.");
            }

            var admin = await userManager.FindByNameAsync(adminUserName);
            if (admin is null)
            {
                admin = new AppUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DisplayName = "Genmon Administrator",
                    PreferredLanguage = adminLanguage
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                {
                    var message = string.Join(", ", result.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Unable to create bootstrap admin user: {message}");
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Administrator"))
            {
                await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }

        if (!await dbContext.Localizations.AnyAsync(cancellationToken))
        {
            dbContext.Localizations.AddRange(
                new LocalizationEntry { Culture = "en", Key = "dashboard.title", Value = "Generators" },
                new LocalizationEntry { Culture = "en", Key = "state.pending", Value = "Pending protocol migration" },
                new LocalizationEntry { Culture = "es", Key = "dashboard.title", Value = "Generadores" },
                new LocalizationEntry { Culture = "es", Key = "state.pending", Value = "Migración de protocolo pendiente" });

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
