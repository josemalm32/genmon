using Genmon.Domain.Common;

namespace Genmon.Domain.Entities;

public sealed class GeneratorStatusSnapshot : TrackableEntity
{
    public Guid GeneratorId { get; set; }
    public Generator? Generator { get; set; }
    public DateTimeOffset CapturedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string OverallState { get; set; } = "Unknown";
    public bool IsRunning { get; set; }
    public bool IsAlarmed { get; set; }
    public string PayloadJson { get; set; } = "{}";
}
