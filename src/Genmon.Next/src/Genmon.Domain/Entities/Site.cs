using Genmon.Domain.Common;

namespace Genmon.Domain.Entities;

public sealed class Site : TrackableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DefaultLanguage { get; set; } = "en";
    public string TimeZoneId { get; set; } = "UTC";
    public ICollection<Generator> Generators { get; set; } = new List<Generator>();
}
