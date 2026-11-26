namespace LigaArmenia.Domain.Rules;

public static class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 128;

    public static (bool IsValid, IReadOnlyList<string> Errors) Validate(string? password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("La contraseña es obligatoria.");
            return (false, errors);
        }

        if (password.Length < MinLength)
            errors.Add($"La contraseña debe tener al menos {MinLength} caracteres.");

        if (password.Length > MaxLength)
            errors.Add($"La contraseña no puede superar los {MaxLength} caracteres.");

        if (!password.Any(char.IsUpper))
            errors.Add("La contraseña debe incluir al menos una letra mayúscula.");

        if (!password.Any(char.IsLower))
            errors.Add("La contraseña debe incluir al menos una letra minúscula.");

        if (!password.Any(char.IsDigit))
            errors.Add("La contraseña debe incluir al menos un número.");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            errors.Add("La contraseña debe incluir al menos un carácter especial.");

        return (errors.Count == 0, errors);
    }

    public static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToLowerInvariant();
    }
}
