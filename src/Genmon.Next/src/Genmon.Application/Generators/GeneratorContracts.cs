using Genmon.Domain.Enums;

namespace Genmon.Application.Generators;

public sealed record GeneratorConnectionRequest(
    ConnectionKind ConnectionKind,
    string? Endpoint,
    int? Port,
    string? SerialPort,
    byte? UnitId,
    int PollingIntervalSeconds,
    string SettingsJson);

public sealed record CreateGeneratorRequest(
    string Name,
    string SiteName,
    ControllerType ControllerType,
    string PreferredLanguage,
    GeneratorConnectionRequest Connection);

public sealed record UpdateGeneratorRequest(
    string Name,
    string SiteName,
    ControllerType ControllerType,
    string PreferredLanguage,
    bool IsEnabled,
    GeneratorConnectionRequest Connection);

public sealed record GeneratorSummaryResponse(
    Guid Id,
    string Name,
    string Slug,
    Guid SiteId,
    string SiteName,
    ControllerType ControllerType,
    string PreferredLanguage,
    bool IsEnabled,
    string CurrentState,
    bool IsRunning,
    bool IsAlarmed,
    DateTimeOffset? LastSeenUtc,
    GeneratorConnectionRequest? PrimaryConnection);
