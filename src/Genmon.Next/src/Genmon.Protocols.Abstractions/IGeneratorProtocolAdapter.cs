using Genmon.Domain.Entities;
using Genmon.Domain.Enums;

namespace Genmon.Protocols.Abstractions;

public interface IGeneratorProtocolAdapter
{
    ControllerType ControllerType { get; }
    Task<GeneratorTelemetry> PollAsync(Generator generator, CancellationToken cancellationToken);
}
