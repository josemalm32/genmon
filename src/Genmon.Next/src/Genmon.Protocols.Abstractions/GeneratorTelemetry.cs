namespace Genmon.Protocols.Abstractions;

public sealed record GeneratorTelemetry(
    DateTimeOffset CapturedAtUtc,
    string OverallState,
    bool IsRunning,
    bool IsAlarmed,
    string PayloadJson);
