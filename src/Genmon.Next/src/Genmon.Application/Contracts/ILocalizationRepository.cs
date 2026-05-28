using Genmon.Domain.Entities;

namespace Genmon.Application.Contracts;

public interface ILocalizationRepository
{
    Task<IReadOnlyList<LocalizationEntry>> ListByCultureAsync(string culture, CancellationToken cancellationToken);
}
