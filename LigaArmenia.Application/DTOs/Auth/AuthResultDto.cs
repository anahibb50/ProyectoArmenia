namespace LigaArmenia.Application.DTOs.Auth;

public enum AuthChannel
{
    WebPortal = 1,
    MobileApi = 2
}

public sealed class AuthResultDto
{
    public bool Succeeded { get; private init; }
    public bool IsLockedOut { get; private init; }
    public bool IsChannelForbidden { get; private init; }
    public string? ErrorMessage { get; private init; }
    public AuthenticatedUserDto? User { get; private init; }

    public static AuthResultDto Success(AuthenticatedUserDto user) => new()
    {
        Succeeded = true,
        User = user
    };

    public static AuthResultDto InvalidCredentials() => new()
    {
        Succeeded = false,
        ErrorMessage = "Correo o contraseña incorrectos."
    };

    public static AuthResultDto LockedOut(TimeSpan remaining) => new()
    {
        Succeeded = false,
        IsLockedOut = true,
        ErrorMessage = $"Demasiados intentos fallidos. Intenta nuevamente en {Math.Max(1, (int)Math.Ceiling(remaining.TotalMinutes))} minuto(s)."
    };

    public static AuthResultDto ChannelForbidden(string message) => new()
    {
        Succeeded = false,
        IsChannelForbidden = true,
        ErrorMessage = message
    };
}
