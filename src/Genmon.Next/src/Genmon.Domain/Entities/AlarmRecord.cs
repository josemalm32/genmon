using Genmon.Domain.Common;
using Genmon.Domain.Enums;

namespace Genmon.Domain.Entities;

public sealed class AlarmRecord : TrackableEntity
{
    public Guid GeneratorId { get; set; }
    public Generator? Generator { get; set; }
    public DateTimeOffset RaisedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClearedAtUtc { get; set; }
    public AlarmSeverity Severity { get; set; } = AlarmSeverity.Warning;
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsAcknowledged { get; set; }
}
