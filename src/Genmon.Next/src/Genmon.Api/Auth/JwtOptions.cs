namespace Genmon.Api.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = "Genmon.Next";
    public string Audience { get; init; } = "Genmon.Next.Clients";
    public string SigningKey { get; init; } = "ReplaceThisDevelopmentKeyWithASecureValue123!";
    public int ExpiresMinutes { get; init; } = 60;
}
