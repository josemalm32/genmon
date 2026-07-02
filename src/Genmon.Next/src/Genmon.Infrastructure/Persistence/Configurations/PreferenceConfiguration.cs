using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.Property(record => record.Key).HasMaxLength(100).IsRequired();
        builder.Property(record => record.Value).HasColumnType("TEXT").IsRequired();
        builder.HasIndex(record => new { record.UserId, record.GeneratorId, record.Scope, record.Key }).IsUnique();
    }
}
