using Genmon.Domain.Enums;
using Genmon.Protocols.Abstractions;

namespace Genmon.Infrastructure.Protocols;

public sealed class GeneratorProtocolAdapterResolver(IEnumerable<IGeneratorProtocolAdapter> adapters) : IGeneratorProtocolAdapterResolver
{
    private readonly IReadOnlyDictionary<ControllerType, IGeneratorProtocolAdapter> _adapters = adapters.ToDictionary(item => item.ControllerType);

    public IGeneratorProtocolAdapter Resolve(ControllerType controllerType)
        => _adapters.TryGetValue(controllerType, out var adapter)
            ? adapter
            : _adapters[ControllerType.Unknown];
}
