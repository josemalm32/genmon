using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class GeneratorConnectionConfiguration : IEntityTypeConfiguration<GeneratorConnection>
{
    public void Configure(EntityTypeBuilder<GeneratorConnection> builder)
    {
        builder.Property(connection => connection.Endpoint).HasMaxLength(255);
        builder.Property(connection => connection.SerialPort).HasMaxLength(255);
        builder.Property(connection => connection.SettingsJson).HasColumnType("TEXT").IsRequired();
        builder.HasOne(connection => connection.Generator)
            .WithMany(generator => generator.Connections)
            .HasForeignKey(connection => connection.GeneratorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
