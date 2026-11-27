namespace LigaArmenia.Domain.Rules;

/// <summary>
/// Definición fija de la temporada 2026-2027.
/// Sénior A y Sénior B no se reconfiguran. Infantil y Femenino nacen sin cupo ni etapas.
/// </summary>
public static class CatalogoTemporadaInicial
{
    public const string NombreTemporada = "2026-2027";
    public const string DisciplinaSenior = "SENIOR";
    public const string DisciplinaInfantil = "INFANTIL";
    public const string DisciplinaFemenino = "FEMENINO";
    public const string FormatoTodosContraTodos = "TODOS_CONTRA_TODOS";
    public const string FormatoCuadrangulares = "CUADRANGULARES";
    public const string FormatoEliminacion = "ELIMINACION";
    public const short PuntosVictoria = 3;
    public const short PuntosEmpate = 1;
    public const short PuntosDerrota = 0;
    public const int MaximoEtapas = 3;
    public const int MaximoNombreEquipo = 140;
    public const int MaximoNombreEtapa = 40;

    public static readonly IReadOnlySet<string> FormatosPermitidos = new HashSet<string>(StringComparer.Ordinal)
    {
        FormatoTodosContraTodos,
        FormatoCuadrangulares,
        FormatoEliminacion
    };

    private static readonly DefinicionEtapa EtapaRegular1 = new(1, "Etapa 1", FormatoTodosContraTodos);
    private static readonly DefinicionEtapa EtapaRegular2 = new(2, "Etapa 2", FormatoTodosContraTodos);
    private static readonly DefinicionEtapa EtapaLiguilla = new(3, "Etapa 3", FormatoCuadrangulares);

    public static IReadOnlyList<DefinicionCategoria> Categorias { get; } =
    [
        new(
            "SENIOR_A",
            "Sénior A",
            DisciplinaSenior,
            'A',
            12,
            PuntosVictoria,
            PuntosEmpate,
            PuntosDerrota,
            [EtapaRegular1, EtapaRegular2, EtapaLiguilla],
            EsConfigurable: false),
        new(
            "SENIOR_B",
            "Sénior B",
            DisciplinaSenior,
            'B',
            14,
            PuntosVictoria,
            PuntosEmpate,
            PuntosDerrota,
            [EtapaRegular1, EtapaRegular2],
            EsConfigurable: false),
        new(
            "INFANTIL",
            "Infantil",
            DisciplinaInfantil,
            null,
            null,
            PuntosVictoria,
            PuntosEmpate,
            PuntosDerrota,
            [],
            EsConfigurable: true),
        new(
            "FEMENINO",
            "Femenino",
            DisciplinaFemenino,
            null,
            null,
            PuntosVictoria,
            PuntosEmpate,
            PuntosDerrota,
            [],
            EsConfigurable: true)
    ];

    public static string NormalizarCodigo(string? codigo)
    {
        return (codigo ?? string.Empty).Trim().ToUpperInvariant();
    }

    public static DefinicionCategoria? Buscar(string? codigo)
    {
        var normalizado = NormalizarCodigo(codigo);
        return Categorias.FirstOrDefault(c =>
            string.Equals(c.Codigo, normalizado, StringComparison.Ordinal));
    }

    public static IReadOnlyList<string> ValidarConfiguracionDivision(
        short cupoEquipos,
        IReadOnlyList<DefinicionEtapa> etapas)
    {
        var errores = new List<string>();

        if (cupoEquipos <= 0)
            errores.Add("El cupo de equipos debe ser mayor que cero.");

        if (etapas.Count == 0)
            errores.Add("La división necesita al menos una etapa.");

        if (etapas.Count > MaximoEtapas)
            errores.Add($"Una categoría admite como máximo {MaximoEtapas} etapas.");

        var numeros = new HashSet<short>();
        foreach (var etapa in etapas)
        {
            if (etapa.Numero is < 1 or > MaximoEtapas)
                errores.Add($"La etapa {etapa.Numero} está fuera del rango 1 a {MaximoEtapas}.");

            if (!numeros.Add(etapa.Numero))
                errores.Add($"La etapa {etapa.Numero} está repetida.");

            var nombre = etapa.Nombre?.Trim() ?? string.Empty;
            if (nombre.Length == 0 || nombre.Length > MaximoNombreEtapa)
                errores.Add($"Cada etapa necesita un nombre de hasta {MaximoNombreEtapa} caracteres.");

            if (!FormatosPermitidos.Contains(etapa.Formato))
                errores.Add($"El formato '{etapa.Formato}' no está permitido.");
        }

        return errores;
    }
}

public sealed record DefinicionCategoria(
    string Codigo,
    string Nombre,
    string Disciplina,
    char? Serie,
    short? CupoEquipos,
    short PuntosVictoria,
    short PuntosEmpate,
    short PuntosDerrota,
    IReadOnlyList<DefinicionEtapa> Etapas,
    bool EsConfigurable);

public sealed record DefinicionEtapa(short Numero, string Nombre, string Formato);
