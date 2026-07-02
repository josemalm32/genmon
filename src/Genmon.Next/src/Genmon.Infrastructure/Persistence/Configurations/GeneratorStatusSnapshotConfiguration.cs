using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class GeneratorStatusSnapshotConfiguration : IEntityTypeConfiguration<GeneratorStatusSnapshot>
{
    public void Configure(EntityTypeBuilder<GeneratorStatusSnapshot> builder)
    {
        builder.Property(snapshot => snapshot.OverallState).HasMaxLength(64).IsRequired();
        builder.Property(snapshot => snapshot.PayloadJson).HasColumnType("TEXT").IsRequired();
        builder.HasIndex(snapshot => new { snapshot.GeneratorId, snapshot.CapturedAtUtc });
    }
}
