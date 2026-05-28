using System.Text.Json;
using Genmon.Domain.Entities;
using Genmon.Domain.Enums;
using Genmon.Protocols.Abstractions;

namespace Genmon.Infrastructure.Protocols;

public sealed class PlaceholderGeneratorProtocolAdapter : IGeneratorProtocolAdapter
{
    public ControllerType ControllerType { get; }

    public PlaceholderGeneratorProtocolAdapter(ControllerType controllerType)
    {
        ControllerType = controllerType;
    }

    public Task<GeneratorTelemetry> PollAsync(Generator generator, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(new
        {
            generator.ControllerType,
            generator.Name,
            generator.PreferredLanguage,
            message = ControllerType == ControllerType.Unknown
                ? "No protocol adapter registered yet."
                : "Protocol adapter scaffolded; telemetry migration pending.",
            connection = generator.Connections
                .OrderByDescending(item => item.IsPrimary)
                .Select(item => new
                {
                    item.ConnectionKind,
                    item.Endpoint,
                    item.Port,
                    item.SerialPort,
                    item.UnitId,
                    item.PollingIntervalSeconds
                })
                .FirstOrDefault()
        });

        return Task.FromResult(new GeneratorTelemetry(
            DateTimeOffset.UtcNow,
            ControllerType == ControllerType.Unknown ? "PendingConfiguration" : "PendingProtocolMigration",
            false,
            false,
            payload));
    }
}
