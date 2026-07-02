using Genmon.Domain.Common;
using Genmon.Domain.Enums;

namespace Genmon.Domain.Entities;

public sealed class Preference : TrackableEntity
{
    public Guid? UserId { get; set; }
    public Guid? GeneratorId { get; set; }
    public Generator? Generator { get; set; }
    public PreferenceScope Scope { get; set; } = PreferenceScope.User;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
