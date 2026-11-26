namespace LigaArmenia.Application.DTOs.Auth;

public sealed class LoginRequestDto
{
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Recordarme { get; set; }
}
