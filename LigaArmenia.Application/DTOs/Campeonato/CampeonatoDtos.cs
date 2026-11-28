namespace LigaArmenia.Application.DTOs.Campeonato;

public sealed class TemporadaResumenDto
{
    public long Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public bool Activa { get; init; }
}

public sealed class TemporadaConfiguradaDto
{
    public long TemporadaId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public bool Activa { get; init; }
    public IReadOnlyList<CategoriaDto> Categorias { get; init; } = [];
}

public sealed class CategoriaDto
{
    public short Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string Disciplina { get; init; } = string.Empty;
    public char? Serie { get; init; }
    public short? CupoEquipos { get; init; }
    public short PuntosVictoria { get; init; }
    public short PuntosEmpate { get; init; }
    public short PuntosDerrota { get; init; }
    public bool EsConfigurable { get; init; }
    public IReadOnlyList<EtapaCategoriaDto> Etapas { get; init; } = [];
}

public sealed class EtapaCategoriaDto
{
    public short Numero { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Formato { get; init; } = string.Empty;
}

public sealed class EtapaSolicitudDto
{
    public short Numero { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Formato { get; init; } = string.Empty;
}

public sealed class EquipoListadoDto
{
    public long Id { get; init; }
    public short CategoriaId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public bool Activo { get; init; }
}

public sealed class PanelCampeonatoDto
{
    public TemporadaConfiguradaDto Temporada { get; init; } = new();
    public string SiguienteTemporada { get; init; } = string.Empty;
    public IReadOnlyList<TemporadaResumenDto> Temporadas { get; init; } = [];
    public IReadOnlyList<EquipoListadoDto> Equipos { get; init; } = [];
}

public sealed class EquipoCreadoDto
{
    public long Id { get; init; }
    public long TemporadaId { get; init; }
    public short CategoriaId { get; init; }
    public string CodigoCategoria { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public char? Serie { get; init; }
}
