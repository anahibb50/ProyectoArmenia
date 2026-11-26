namespace LigaArmenia.Domain.Constants;

public static class SystemRoles
{
    public const string Administrador = "ADMINISTRADOR";
    public const string Vocal = "VOCAL";
    public const string ComisionTecnica = "COMISION_TECNICA";
    public const string ComisionDisciplinaria = "COMISION_DISCIPLINARIA";
    public const string Directivo = "DIRECTIVO";

    public static readonly IReadOnlyCollection<string> WebPortalAllowedRoles = new[]
    {
        Administrador,
        ComisionTecnica,
        ComisionDisciplinaria,
        Directivo
    };

    public static readonly IReadOnlyCollection<string> MobileAllowedRoles = new[]
    {
        Vocal,
        Administrador
    };
}
