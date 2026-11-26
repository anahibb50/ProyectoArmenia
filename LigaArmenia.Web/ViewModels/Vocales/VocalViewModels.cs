using System.ComponentModel.DataAnnotations;
using LigaArmenia.Application.DTOs.Vocales;

namespace LigaArmenia.Web.ViewModels.Vocales;

public sealed class VocalIndexViewModel
{
    public string? Busqueda { get; set; }
    public bool? Activo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalFiltered { get; set; }
    public int TotalPages { get; set; } = 1;
    public int TotalRegistrados { get; set; }
    public int TotalActivos { get; set; }
    public int TotalInactivos { get; set; }
    public IReadOnlyList<VocalListItemDto> Vocales { get; set; } = Array.Empty<VocalListItemDto>();
}

public sealed class VocalEditViewModel
{
    public long Id { get; set; }
    public long? UsuarioId { get; set; }
    public string? Correo { get; set; }
    public bool Activo { get; set; }
    public bool UsuarioActivo { get; set; }
    public bool TieneRolVocal { get; set; }
    public DateTime CreadoEn { get; set; }

    [Required(ErrorMessage = "Los nombres completos son obligatorios.")]
    [StringLength(180, ErrorMessage = "Los nombres no pueden superar los 180 caracteres.")]
    [Display(Name = "Nombres completos")]
    public string Nombres { get; set; } = string.Empty;

    [StringLength(35, ErrorMessage = "El teléfono no puede superar los 35 caracteres.")]
    [Display(Name = "Teléfono (opcional)")]
    public string? Telefono { get; set; }
}
