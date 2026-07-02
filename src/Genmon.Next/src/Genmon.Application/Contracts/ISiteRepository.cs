using Genmon.Domain.Entities;

namespace Genmon.Application.Contracts;

public interface ISiteRepository
{
    Task<Site> GetOrCreateAsync(string name, string language, CancellationToken cancellationToken);
}
