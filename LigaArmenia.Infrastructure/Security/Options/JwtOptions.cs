namespace LigaArmenia.Infrastructure.Security.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;

    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Key) &&
        Key.Length >= 32 &&
        !string.IsNullOrWhiteSpace(Issuer) &&
        !string.IsNullOrWhiteSpace(Audience) &&
        ExpirationMinutes > 0;
}
