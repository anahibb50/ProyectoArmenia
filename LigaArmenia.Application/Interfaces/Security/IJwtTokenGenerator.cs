using LigaArmenia.Application.DTOs.Auth;

namespace LigaArmenia.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    bool IsConfigured { get; }
    JwtTokenResultDto GenerateToken(AuthenticatedUserDto user);
}
