using Genmon.Application.Contracts;
using Genmon.Domain.Entities;
using Genmon.Infrastructure.Persistence;
using Genmon.Protocols.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Genmon.Infrastructure.Background;

public sealed class GeneratorPollingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<GeneratorPollingBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollGeneratorsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Generator polling iteration failed.");
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }

    private async Task PollGeneratorsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGeneratorRepository>();
        var resolver = scope.ServiceProvider.GetRequiredService<IGeneratorProtocolAdapterResolver>();
        var dbContext = scope.ServiceProvider.GetRequiredService<GenmonDbContext>();
        var generators = await repository.ListEnabledAsync(cancellationToken);

        foreach (var generator in generators)
        {
            var adapter = resolver.Resolve(generator.ControllerType);
            var telemetry = await adapter.PollAsync(generator, cancellationToken);

            generator.CurrentState = telemetry.OverallState;
            generator.IsRunning = telemetry.IsRunning;
            generator.IsAlarmed = telemetry.IsAlarmed;
            generator.LastSeenUtc = telemetry.CapturedAtUtc;
            generator.CurrentPayloadJson = telemetry.PayloadJson;
            generator.UpdatedUtc = DateTimeOffset.UtcNow;

            dbContext.GeneratorStatusSnapshots.Add(new GeneratorStatusSnapshot
            {
                GeneratorId = generator.Id,
                CapturedAtUtc = telemetry.CapturedAtUtc,
                OverallState = telemetry.OverallState,
                IsRunning = telemetry.IsRunning,
                IsAlarmed = telemetry.IsAlarmed,
                PayloadJson = telemetry.PayloadJson
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var cutoff = DateTimeOffset.UtcNow.AddDays(-30);
        await dbContext.GeneratorStatusSnapshots
            .Where(snapshot => snapshot.CapturedAtUtc < cutoff)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
