using Genmon.Application.Contracts;
using Genmon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Genmon.Infrastructure.Persistence.Repositories;

public sealed class SiteRepository(GenmonDbContext dbContext) : ISiteRepository
{
    public async Task<Site> GetOrCreateAsync(string name, string language, CancellationToken cancellationToken)
    {
        var normalizedName = string.IsNullOrWhiteSpace(name) ? "Default" : name.Trim();
        var site = await dbContext.Sites.FirstOrDefaultAsync(item => item.Name == normalizedName, cancellationToken);
        if (site is not null)
        {
            return site;
        }

        site = new Site
        {
            Name = normalizedName,
            DefaultLanguage = string.IsNullOrWhiteSpace(language) ? "en" : language.Trim().ToLowerInvariant()
        };

        await dbContext.Sites.AddAsync(site, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return site;
    }
}
