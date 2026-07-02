using Genmon.Domain.Common;
using Genmon.Domain.Enums;

namespace Genmon.Domain.Entities;

public sealed class GeneratorConnection : TrackableEntity
{
    public Guid GeneratorId { get; set; }
    public Generator? Generator { get; set; }
    public ConnectionKind ConnectionKind { get; set; } = ConnectionKind.Tcp;
    public string? Endpoint { get; set; }
    public int? Port { get; set; }
    public string? SerialPort { get; set; }
    public byte? UnitId { get; set; }
    public int PollingIntervalSeconds { get; set; } = 15;
    public string SettingsJson { get; set; } = "{}";
    public bool IsPrimary { get; set; } = true;
}
