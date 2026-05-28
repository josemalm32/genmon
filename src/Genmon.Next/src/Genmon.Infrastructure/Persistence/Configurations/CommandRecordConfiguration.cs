using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Genmon.Infrastructure.Persistence.Configurations;

public sealed class CommandRecordConfiguration : IEntityTypeConfiguration<CommandRecord>
{
    public void Configure(EntityTypeBuilder<CommandRecord> builder)
    {
        builder.Property(record => record.CommandName).HasMaxLength(100).IsRequired();
        builder.Property(record => record.ParametersJson).HasColumnType("TEXT").IsRequired();
    }
}
