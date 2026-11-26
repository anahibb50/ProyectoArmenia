namespace LigaArmenia.Application.Interfaces.Security;

public interface ICurrentUserService
{
    long? UserId { get; }
    string? Email { get; }
    string? FullName { get; }
    bool IsAuthenticated { get; }
}
