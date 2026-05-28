using Genmon.Application.Contracts;

namespace Genmon.Application.Dashboard;

public sealed class DashboardService(IGeneratorRepository generatorRepository)
{
    public async Task<DashboardSummaryResponse> GetAsync(CancellationToken cancellationToken)
    {
        var generators = await generatorRepository.ListAsync(cancellationToken);
        var list = generators
            .Select(generator => new DashboardGeneratorResponse(
                generator.Id,
                generator.Name,
                generator.Site?.Name ?? string.Empty,
                generator.CurrentState,
                generator.IsRunning,
                generator.IsAlarmed,
                generator.LastSeenUtc,
                generator.PreferredLanguage))
            .ToList();

        return new DashboardSummaryResponse(
            list.Count,
            generators.Count(generator => generator.IsEnabled),
            generators.Count(generator => generator.IsRunning),
            generators.Count(generator => generator.IsAlarmed),
            generators.Count(generator => generator.LastSeenUtc is not null && generator.LastSeenUtc >= DateTimeOffset.UtcNow.AddMinutes(-2)),
            list);
    }
}
