using LigaArmenia.Application.DTOs.Auth;

namespace LigaArmenia.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResultDto> AuthenticateAsync(
        LoginRequestDto request,
        AuthChannel channel,
        string? clientIp,
        CancellationToken cancellationToken = default);
}
