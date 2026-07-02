using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class MaintenanceLogRecordConfiguration : IEntityTypeConfiguration<MaintenanceLogRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceLogRecord> builder)
    {
        builder.Property(record => record.Title).HasMaxLength(200).IsRequired();
        builder.Property(record => record.Notes).HasMaxLength(4096);
        builder.Property(record => record.PerformedBy).HasMaxLength(200);
    }
}
