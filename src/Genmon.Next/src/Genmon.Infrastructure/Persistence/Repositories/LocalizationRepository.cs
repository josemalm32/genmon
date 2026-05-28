using Genmon.Application.Contracts;
using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Genmon.Infrastructure.Persistence.Repositories;

public sealed class LocalizationRepository(GenmonDbContext dbContext) : ILocalizationRepository
{
    public async Task<IReadOnlyList<LocalizationEntry>> ListByCultureAsync(string culture, CancellationToken cancellationToken)
    {
        var normalizedCulture = string.IsNullOrWhiteSpace(culture) ? "en" : culture.Trim().ToLowerInvariant();
        var items = await dbContext.Localizations
            .Where(item => item.Culture == normalizedCulture)
            .OrderBy(item => item.Key)
            .ToListAsync(cancellationToken);

        if (items.Count > 0 || normalizedCulture == "en")
        {
            return items;
        }

        return await dbContext.Localizations
            .Where(item => item.Culture == "en")
            .OrderBy(item => item.Key)
            .ToListAsync(cancellationToken);
    }
}
