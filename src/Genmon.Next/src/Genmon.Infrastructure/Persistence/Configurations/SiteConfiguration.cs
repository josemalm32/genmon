using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.Property(site => site.Name).HasMaxLength(200).IsRequired();
        builder.Property(site => site.DefaultLanguage).HasMaxLength(12).IsRequired();
        builder.Property(site => site.TimeZoneId).HasMaxLength(100).IsRequired();
        builder.HasIndex(site => site.Name).IsUnique();
    }
}
