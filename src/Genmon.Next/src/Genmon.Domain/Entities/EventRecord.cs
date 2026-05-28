using Genmon.Domain.Common;
using Genmon.Domain.Enums;

namespace Genmon.Domain.Entities;

public sealed class EventRecord : TrackableEntity
{
    public Guid GeneratorId { get; set; }
    public Generator? Generator { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string EventType { get; set; } = string.Empty;
    public AlarmSeverity Severity { get; set; } = AlarmSeverity.Info;
    public string Message { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
}
