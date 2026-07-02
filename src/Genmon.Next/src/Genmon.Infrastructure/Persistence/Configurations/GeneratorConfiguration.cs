using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class GeneratorConfiguration : IEntityTypeConfiguration<Generator>
{
    public void Configure(EntityTypeBuilder<Generator> builder)
    {
        builder.Property(generator => generator.Name).HasMaxLength(200).IsRequired();
        builder.Property(generator => generator.Slug).HasMaxLength(200).IsRequired();
        builder.Property(generator => generator.PreferredLanguage).HasMaxLength(12).IsRequired();
        builder.Property(generator => generator.CurrentState).HasMaxLength(64).IsRequired();
        builder.Property(generator => generator.CurrentPayloadJson).HasColumnType("TEXT");
        builder.HasIndex(generator => generator.Slug).IsUnique();
        builder.HasOne(generator => generator.Site)
            .WithMany(site => site.Generators)
            .HasForeignKey(generator => generator.SiteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
