namespace LigaArmenia.Application.DTOs.Auth;

public sealed class ApiLoginRequestDto
{
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class JwtTokenResultDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public DateTime ExpiresAtUtc { get; init; }
}

public sealed class ApiLoginResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public DateTime ExpiresAtUtc { get; init; }
    public UserProfileApiDto Usuario { get; init; } = new();
}

public sealed class UserProfileApiDto
{
    public long Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public bool Activo { get; init; }
    public long? VocalId { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Permisos { get; init; } = Array.Empty<string>();
}
