using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LigaArmenia.Infrastructure.Security;

public sealed class AspNetPasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public string HashPassword(Usuario usuario, string plainPassword)
    {
        return _passwordHasher.HashPassword(usuario, plainPassword);
    }

    public bool VerifyPassword(Usuario usuario, string hashedPassword, string providedPassword, out bool needsRehash)
    {
        needsRehash = false;

        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(providedPassword))
            return false;

        var result = _passwordHasher.VerifyHashedPassword(usuario, hashedPassword, providedPassword);
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            needsRehash = true;
            return true;
        }

        return result == PasswordVerificationResult.Success;
    }
}
