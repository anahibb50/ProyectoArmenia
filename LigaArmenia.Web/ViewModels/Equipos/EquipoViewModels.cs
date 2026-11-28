using System.ComponentModel.DataAnnotations;

namespace LigaArmenia.Web.ViewModels.Equipos;

public sealed class EquiposIndexViewModel
{
    public string NombreTemporada { get; set; } = string.Empty;
    public bool EsTemporadaActual { get; set; }
    public IReadOnlyList<TemporadaOpcionViewModel> Temporadas { get; set; } = [];
    public string CategoriaSeleccionada { get; set; } = string.Empty;
    public int TotalEquipos { get; set; }
    public IReadOnlyList<EquipoFilaViewModel> Equipos { get; set; } = [];
    public IReadOnlyList<CategoriaOpcionViewModel> Categorias { get; set; } = [];
}

public sealed class TemporadaOpcionViewModel
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activa { get; set; }
}

public sealed class CategoriaOpcionViewModel
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool CupoLleno { get; set; }
}

public sealed class EquipoFilaViewModel
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CodigoCategoria { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int Jugadores { get; set; }
    public bool Activo { get; set; }
}

public sealed class RegistrarEquipoEnTemporadaViewModel
{
    [Required(ErrorMessage = "Selecciona la temporada.")]
    public string Temporada { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona la categoría.")]
    public string CodigoCategoria { get; set; } = string.Empty;

    public string? CategoriaFiltro { get; set; }

    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(140)]
    public string Nombre { get; set; } = string.Empty;
}

public sealed class NominaViewModel
{
    public long EquipoId { get; set; }
    public string NombreEquipo { get; set; } = string.Empty;
    public string NombreCategoria { get; set; } = string.Empty;
    public string NombreTemporada { get; set; } = string.Empty;
    public IReadOnlyList<JugadorFilaViewModel> Activos { get; set; } = [];
    public IReadOnlyList<JugadorFilaViewModel> Bajas { get; set; } = [];
}

public sealed class JugadorFilaViewModel
{
    public long MembresiaId { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public DateOnly? FechaNacimiento { get; set; }
    public int? Edad { get; set; }
    public bool MayorDe50 { get; set; }
    public bool ConsideradoJuvenil { get; set; }
    public bool Habilitado { get; set; }
    public DateOnly FechaAlta { get; set; }
    public DateOnly? FechaBaja { get; set; }
}

public sealed class AgregarJugadorViewModel
{
    public long EquipoId { get; set; }

    [Required(ErrorMessage = "El documento es obligatorio.")]
    [StringLength(35)]
    [Display(Name = "Documento")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del jugador es obligatorio.")]
    [StringLength(180)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateOnly? FechaNacimiento { get; set; }

    [Display(Name = "Considerado juvenil")]
    public bool ConsideradoJuvenil { get; set; }

    [Display(Name = "Habilitado")]
    public bool Habilitado { get; set; } = true;
}
