using Genmon.Domain.Common;

namespace Genmon.Domain.Entities;

public sealed class LocalizationEntry : TrackableEntity
{
    public string Culture { get; set; } = "en";
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
