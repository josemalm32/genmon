using Genmon.Application.Contracts;

namespace Genmon.Application.Localization;

public sealed class LocalizationService(ILocalizationRepository localizationRepository)
{
    public async Task<IReadOnlyDictionary<string, string>> GetCultureAsync(string culture, CancellationToken cancellationToken)
    {
        var normalizedCulture = string.IsNullOrWhiteSpace(culture) ? "en" : culture.Trim().ToLowerInvariant();
        var items = await localizationRepository.ListByCultureAsync(normalizedCulture, cancellationToken);
        return items.ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);
    }
}
