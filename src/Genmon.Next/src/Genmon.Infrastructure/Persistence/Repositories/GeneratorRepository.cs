using Genmon.Application.Contracts;
using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Genmon.Infrastructure.Persistence.Repositories;

public sealed class GeneratorRepository(GenmonDbContext dbContext) : IGeneratorRepository
{
    public async Task<IReadOnlyList<Generator>> ListAsync(CancellationToken cancellationToken)
        => await dbContext.Generators
            .Include(generator => generator.Site)
            .Include(generator => generator.Connections)
            .OrderBy(generator => generator.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Generator>> ListEnabledAsync(CancellationToken cancellationToken)
        => await dbContext.Generators
            .Include(generator => generator.Site)
            .Include(generator => generator.Connections)
            .Where(generator => generator.IsEnabled)
            .OrderBy(generator => generator.Name)
            .ToListAsync(cancellationToken);

    public async Task<Generator?> GetAsync(Guid id, CancellationToken cancellationToken)
        => await dbContext.Generators
            .Include(generator => generator.Site)
            .Include(generator => generator.Connections)
            .FirstOrDefaultAsync(generator => generator.Id == id, cancellationToken);

    public Task AddAsync(Generator generator, CancellationToken cancellationToken)
        => dbContext.Generators.AddAsync(generator, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
