using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class EventRecordConfiguration : IEntityTypeConfiguration<EventRecord>
{
    public void Configure(EntityTypeBuilder<EventRecord> builder)
    {
        builder.Property(record => record.EventType).HasMaxLength(100).IsRequired();
        builder.Property(record => record.Message).HasMaxLength(1024).IsRequired();
        builder.Property(record => record.PayloadJson).HasColumnType("TEXT").IsRequired();
    }
}
