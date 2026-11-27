using System.ComponentModel.DataAnnotations;

namespace LigaArmenia.Web.ViewModels.Campeonato;

public sealed class CampeonatoIndexViewModel
{
    public long TemporadaId { get; set; }
    public string NombreTemporada { get; set; } = string.Empty;
    public IReadOnlyList<CategoriaPanelViewModel> Categorias { get; set; } = [];
}

public sealed class CategoriaPanelViewModel
{
    public short Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public string? Serie { get; set; }
    public short? CupoEquipos { get; set; }
    public int EquiposRegistrados { get; set; }
    public short PuntosVictoria { get; set; }
    public short PuntosEmpate { get; set; }
    public short PuntosDerrota { get; set; }
    public bool EsConfigurable { get; set; }
    public bool CupoLleno { get; set; }
    public IReadOnlyList<EtapaPanelViewModel> Etapas { get; set; } = [];
    public IReadOnlyList<EquipoPanelViewModel> Equipos { get; set; } = [];
}

public sealed class EtapaPanelViewModel
{
    public short Numero { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Formato { get; set; } = string.Empty;
}

public sealed class EquipoPanelViewModel
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public sealed class RegistrarEquipoViewModel
{
    [Required(ErrorMessage = "Selecciona la categoría.")]
    public string CodigoCategoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(140, ErrorMessage = "El nombre del equipo no puede superar los 140 caracteres.")]
    [Display(Name = "Nombre del equipo")]
    public string Nombre { get; set; } = string.Empty;
}

public sealed class ConfigurarDivisionViewModel
{
    public string Codigo { get; set; } = string.Empty;
    public string NombreCategoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indica el cupo de equipos.")]
    [Range(1, 99, ErrorMessage = "El cupo debe estar entre 1 y 99 equipos.")]
    [Display(Name = "Cupo de equipos")]
    public short? CupoEquipos { get; set; }

    public List<EtapaFormViewModel> Etapas { get; set; } =
    [
        new() { Numero = 1, Formato = "TODOS_CONTRA_TODOS" },
        new() { Numero = 2, Formato = "TODOS_CONTRA_TODOS" },
        new() { Numero = 3, Formato = "TODOS_CONTRA_TODOS" }
    ];
}

public sealed class EtapaFormViewModel
{
    public short Numero { get; set; }

    [StringLength(40, ErrorMessage = "El nombre de la etapa no puede superar los 40 caracteres.")]
    [Display(Name = "Nombre")]
    public string? Nombre { get; set; }

    [Display(Name = "Formato")]
    public string Formato { get; set; } = "TODOS_CONTRA_TODOS";
}
