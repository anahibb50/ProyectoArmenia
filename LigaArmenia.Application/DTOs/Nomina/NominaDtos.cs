namespace LigaArmenia.Application.DTOs.Nomina;

public sealed class JugadorNominaDto
{
    public long MembresiaId { get; init; }
    public long JugadorId { get; init; }
    public string Documento { get; init; } = string.Empty;
    public string Nombres { get; init; } = string.Empty;
    public DateOnly? FechaNacimiento { get; init; }
    public int? Edad { get; init; }
    public bool MayorDe50 { get; init; }
    public bool ConsideradoJuvenil { get; init; }
    public bool Habilitado { get; init; }
    public DateOnly FechaAlta { get; init; }
    public DateOnly? FechaBaja { get; init; }
}

public sealed class NominaEquipoDto
{
    public long EquipoId { get; init; }
    public string NombreEquipo { get; init; } = string.Empty;
    public string NombreCategoria { get; init; } = string.Empty;
    public string NombreTemporada { get; init; } = string.Empty;
    public bool TemporadaActiva { get; init; }
    public IReadOnlyList<JugadorNominaDto> Jugadores { get; init; } = [];
}
