using Genmon.Application.Contracts;
using Genmon.Domain.Entities;

namespace Genmon.Application.Generators;

public sealed class GeneratorManagementService(
    IGeneratorRepository generatorRepository,
    ISiteRepository siteRepository)
{
    public async Task<IReadOnlyList<GeneratorSummaryResponse>> ListAsync(CancellationToken cancellationToken)
    {
        var generators = await generatorRepository.ListAsync(cancellationToken);
        return generators.Select(Map).ToList();
    }

    public async Task<GeneratorSummaryResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var generator = await generatorRepository.GetAsync(id, cancellationToken);
        return generator is null ? null : Map(generator);
    }

    public async Task<GeneratorSummaryResponse> CreateAsync(CreateGeneratorRequest request, CancellationToken cancellationToken)
    {
        var site = await siteRepository.GetOrCreateAsync(request.SiteName, request.PreferredLanguage, cancellationToken);

        var generator = new Generator
        {
            SiteId = site.Id,
            Site = site,
            Name = request.Name.Trim(),
            Slug = Slugify(request.Name),
            ControllerType = request.ControllerType,
            PreferredLanguage = NormalizeLanguage(request.PreferredLanguage)
        };

        generator.Connections.Add(new GeneratorConnection
        {
            ConnectionKind = request.Connection.ConnectionKind,
            Endpoint = request.Connection.Endpoint?.Trim(),
            Port = request.Connection.Port,
            SerialPort = request.Connection.SerialPort?.Trim(),
            UnitId = request.Connection.UnitId,
            PollingIntervalSeconds = Math.Max(5, request.Connection.PollingIntervalSeconds),
            SettingsJson = NormalizeJson(request.Connection.SettingsJson),
            IsPrimary = true
        });

        await generatorRepository.AddAsync(generator, cancellationToken);
        await generatorRepository.SaveChangesAsync(cancellationToken);
        return Map(generator);
    }

    public async Task<GeneratorSummaryResponse?> UpdateAsync(Guid id, UpdateGeneratorRequest request, CancellationToken cancellationToken)
    {
        var generator = await generatorRepository.GetAsync(id, cancellationToken);
        if (generator is null)
        {
            return null;
        }

        var site = await siteRepository.GetOrCreateAsync(request.SiteName, request.PreferredLanguage, cancellationToken);
        generator.Name = request.Name.Trim();
        generator.Slug = Slugify(request.Name);
        generator.SiteId = site.Id;
        generator.Site = site;
        generator.ControllerType = request.ControllerType;
        generator.PreferredLanguage = NormalizeLanguage(request.PreferredLanguage);
        generator.IsEnabled = request.IsEnabled;
        generator.UpdatedUtc = DateTimeOffset.UtcNow;

        var connection = generator.Connections.OrderByDescending(item => item.IsPrimary).ThenBy(item => item.CreatedUtc).FirstOrDefault();
        if (connection is null)
        {
            connection = new GeneratorConnection { GeneratorId = generator.Id, IsPrimary = true };
            generator.Connections.Add(connection);
        }

        connection.ConnectionKind = request.Connection.ConnectionKind;
        connection.Endpoint = request.Connection.Endpoint?.Trim();
        connection.Port = request.Connection.Port;
        connection.SerialPort = request.Connection.SerialPort?.Trim();
        connection.UnitId = request.Connection.UnitId;
        connection.PollingIntervalSeconds = Math.Max(5, request.Connection.PollingIntervalSeconds);
        connection.SettingsJson = NormalizeJson(request.Connection.SettingsJson);
        connection.UpdatedUtc = DateTimeOffset.UtcNow;

        await generatorRepository.SaveChangesAsync(cancellationToken);
        return Map(generator);
    }

    private static GeneratorSummaryResponse Map(Generator generator)
    {
        var primaryConnection = generator.Connections
            .OrderByDescending(item => item.IsPrimary)
            .ThenBy(item => item.CreatedUtc)
            .Select(connection => new GeneratorConnectionRequest(
                connection.ConnectionKind,
                connection.Endpoint,
                connection.Port,
                connection.SerialPort,
                connection.UnitId,
                connection.PollingIntervalSeconds,
                connection.SettingsJson))
            .FirstOrDefault();

        return new GeneratorSummaryResponse(
            generator.Id,
            generator.Name,
            generator.Slug,
            generator.SiteId,
            generator.Site?.Name ?? string.Empty,
            generator.ControllerType,
            generator.PreferredLanguage,
            generator.IsEnabled,
            generator.CurrentState,
            generator.IsRunning,
            generator.IsAlarmed,
            generator.LastSeenUtc,
            primaryConnection);
    }

    private static string Slugify(string value)
    {
        var chars = value.Trim().ToLowerInvariant().Select(ch => char.IsLetterOrDigit(ch) ? ch : '-').ToArray();
        var slug = string.Join("-", new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(slug) ? Guid.NewGuid().ToString("N") : slug;
    }

    private static string NormalizeLanguage(string value) => string.IsNullOrWhiteSpace(value) ? "en" : value.Trim().ToLowerInvariant();

    private static string NormalizeJson(string value) => string.IsNullOrWhiteSpace(value) ? "{}" : value.Trim();
}
