using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Genmon.Infrastructure.Persistence;

public sealed class GenmonDbContextFactory : IDesignTimeDbContextFactory<GenmonDbContext>
{
    public GenmonDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GenmonDbContext>();
        var dataDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "src/Genmon.Api/App_Data"));
        Directory.CreateDirectory(dataDirectory);
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(dataDirectory, "genmon-next.db")}");
        return new GenmonDbContext(optionsBuilder.Options);
    }
}
