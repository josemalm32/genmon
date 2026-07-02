using Genmon.Domain.Enums;

namespace Genmon.Protocols.Abstractions;

public interface IGeneratorProtocolAdapterResolver
{
    IGeneratorProtocolAdapter Resolve(ControllerType controllerType);
}
