using Genmon.Domain.Common;
using Genmon.Domain.Enums;

namespace Genmon.Domain.Entities;

public sealed class Generator : TrackableEntity
{
    public Guid SiteId { get; set; }
    public Site? Site { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ControllerType ControllerType { get; set; } = ControllerType.Unknown;
    public string PreferredLanguage { get; set; } = "en";
    public bool IsEnabled { get; set; } = true;
    public string CurrentState { get; set; } = "PendingConfiguration";
    public bool IsRunning { get; set; }
    public bool IsAlarmed { get; set; }
    public DateTimeOffset? LastSeenUtc { get; set; }
    public string? CurrentPayloadJson { get; set; }
    public ICollection<GeneratorConnection> Connections { get; set; } = new List<GeneratorConnection>();
    public ICollection<GeneratorStatusSnapshot> StatusSnapshots { get; set; } = new List<GeneratorStatusSnapshot>();
    public ICollection<EventRecord> Events { get; set; } = new List<EventRecord>();
    public ICollection<AlarmRecord> Alarms { get; set; } = new List<AlarmRecord>();
    public ICollection<MaintenanceLogRecord> MaintenanceLogs { get; set; } = new List<MaintenanceLogRecord>();
    public ICollection<CommandRecord> Commands { get; set; } = new List<CommandRecord>();
}
