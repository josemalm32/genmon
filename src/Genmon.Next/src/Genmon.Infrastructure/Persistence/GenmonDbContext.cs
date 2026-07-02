using Genmon.Domain.Entities;
using Genmon.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Genmon.Infrastructure.Persistence;

public sealed class GenmonDbContext(DbContextOptions<GenmonDbContext> options)
    : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Generator> Generators => Set<Generator>();
    public DbSet<GeneratorConnection> GeneratorConnections => Set<GeneratorConnection>();
    public DbSet<GeneratorStatusSnapshot> GeneratorStatusSnapshots => Set<GeneratorStatusSnapshot>();
    public DbSet<EventRecord> Events => Set<EventRecord>();
    public DbSet<AlarmRecord> Alarms => Set<AlarmRecord>();
    public DbSet<MaintenanceLogRecord> MaintenanceLogs => Set<MaintenanceLogRecord>();
    public DbSet<CommandRecord> Commands => Set<CommandRecord>();
    public DbSet<Preference> Preferences => Set<Preference>();
    public DbSet<LocalizationEntry> Localizations => Set<LocalizationEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(GenmonDbContext).Assembly);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var createdProperty = entityType.FindProperty("CreatedUtc");
            if (createdProperty is not null)
            {
                createdProperty.SetDefaultValueSql("CURRENT_TIMESTAMP");
            }
        }
    }
}
