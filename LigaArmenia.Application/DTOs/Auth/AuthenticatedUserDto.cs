namespace LigaArmenia.Application.DTOs.Auth;

public sealed class AuthenticatedUserDto
{
    public long Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public long? VocalId { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Permisos { get; init; } = Array.Empty<string>();
}
