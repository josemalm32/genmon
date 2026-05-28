namespace Genmon.Application.Dashboard;

public sealed record DashboardSummaryResponse(
    int TotalGenerators,
    int EnabledGenerators,
    int RunningGenerators,
    int AlarmedGenerators,
    int OnlineGenerators,
    IReadOnlyList<DashboardGeneratorResponse> Generators);

public sealed record DashboardGeneratorResponse(
    Guid Id,
    string Name,
    string SiteName,
    string State,
    bool IsRunning,
    bool IsAlarmed,
    DateTimeOffset? LastSeenUtc,
    string PreferredLanguage);
