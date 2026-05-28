using Genmon.Domain.Common;
using Genmon.Domain.Enums;

namespace Genmon.Domain.Entities;

public sealed class CommandRecord : TrackableEntity
{
    public Guid GeneratorId { get; set; }
    public Generator? Generator { get; set; }
    public Guid? RequestedByUserId { get; set; }
    public DateTimeOffset RequestedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public string CommandName { get; set; } = string.Empty;
    public CommandStatus Status { get; set; } = CommandStatus.Pending;
    public string ParametersJson { get; set; } = "{}";
}
