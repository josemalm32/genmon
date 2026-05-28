using Genmon.Domain.Enums;
using Genmon.Protocols.Abstractions;

namespace Genmon.Infrastructure.Protocols;

public sealed class GeneratorProtocolAdapterResolver(IEnumerable<IGeneratorProtocolAdapter> adapters) : IGeneratorProtocolAdapterResolver
{
    private readonly IReadOnlyDictionary<ControllerType, IGeneratorProtocolAdapter> _adapters = BuildAdapterMap(adapters);

    public IGeneratorProtocolAdapter Resolve(ControllerType controllerType)
        => _adapters.TryGetValue(controllerType, out var adapter)
            ? adapter
            : _adapters[ControllerType.Unknown];

    private static IReadOnlyDictionary<ControllerType, IGeneratorProtocolAdapter> BuildAdapterMap(IEnumerable<IGeneratorProtocolAdapter> adapters)
    {
        var groups = adapters.GroupBy(item => item.ControllerType).ToList();
        var duplicates = groups.Where(group => group.Count() > 1).Select(group => group.Key).ToList();

        if (duplicates.Count > 0)
        {
            throw new InvalidOperationException(
                $"Multiple protocol adapters were registered for: {string.Join(", ", duplicates)}.");
        }

        return groups.ToDictionary(group => group.Key, group => group.Single());
    }
}
