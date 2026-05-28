using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Genmon.Infrastructure.Persistence;

public sealed class GenmonDbContextFactory : IDesignTimeDbContextFactory<GenmonDbContext>
{
    public GenmonDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GenmonDbContext>();
        var dataDirectory = Path.Combine(ResolveApiContentRoot(), "App_Data");
        Directory.CreateDirectory(dataDirectory);
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(dataDirectory, "genmon-next.db")}");
        return new GenmonDbContext(optionsBuilder.Options);
    }

    private static string ResolveApiContentRoot()
    {
        var overridePath = Environment.GetEnvironmentVariable("GENMON_API_CONTENTROOT");
        if (!string.IsNullOrWhiteSpace(overridePath))
        {
            return Path.GetFullPath(overridePath);
        }

        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (currentDirectory is not null)
        {
            var candidate = Path.Combine(currentDirectory.FullName, "src", "Genmon.Api");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("Unable to locate src/Genmon.Api for design-time database creation. Set GENMON_API_CONTENTROOT to override the path.");
    }
}
