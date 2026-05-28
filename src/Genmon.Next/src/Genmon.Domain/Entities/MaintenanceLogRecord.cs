using Genmon.Domain.Common;

namespace Genmon.Domain.Entities;

public sealed class MaintenanceLogRecord : TrackableEntity
{
    public Guid GeneratorId { get; set; }
    public Generator? Generator { get; set; }
    public DateTimeOffset LoggedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal? Hours { get; set; }
    public string? PerformedBy { get; set; }
}
