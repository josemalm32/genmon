using Genmon.Domain.Entities;

namespace Genmon.Application.Contracts;

public interface IGeneratorRepository
{
    Task<IReadOnlyList<Generator>> ListAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Generator>> ListEnabledAsync(CancellationToken cancellationToken);
    Task<Generator?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Generator generator, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
