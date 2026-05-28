using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class AlarmRecordConfiguration : IEntityTypeConfiguration<AlarmRecord>
{
    public void Configure(EntityTypeBuilder<AlarmRecord> builder)
    {
        builder.Property(record => record.Code).HasMaxLength(100).IsRequired();
        builder.Property(record => record.Message).HasMaxLength(1024).IsRequired();
    }
}
