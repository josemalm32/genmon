using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class LocalizationEntryConfiguration : IEntityTypeConfiguration<LocalizationEntry>
{
    public void Configure(EntityTypeBuilder<LocalizationEntry> builder)
    {
        builder.Property(record => record.Culture).HasMaxLength(12).IsRequired();
        builder.Property(record => record.Key).HasMaxLength(100).IsRequired();
        builder.Property(record => record.Value).HasMaxLength(1024).IsRequired();
        builder.HasIndex(record => new { record.Culture, record.Key }).IsUnique();
    }
}
