using LigaArmenia.Domain.Entities;

namespace LigaArmenia.Application.Interfaces.Security;

public interface IPasswordHasherService
{
    string HashPassword(Usuario usuario, string plainPassword);
    bool VerifyPassword(Usuario usuario, string hashedPassword, string providedPassword, out bool needsRehash);
}
