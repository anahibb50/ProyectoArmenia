using System.ComponentModel.DataAnnotations;
using LigaArmenia.Application.DTOs.Users;

namespace LigaArmenia.Web.ViewModels.Users;

public sealed class RoleOptionViewModel
{
    public long Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
}

public sealed class UsuarioIndexViewModel
{
    public string? Busqueda { get; set; }
    public long? RolId { get; set; }
    public bool? Activo { get; set; }
    public IReadOnlyList<UsuarioListItemDto> Usuarios { get; set; } = Array.Empty<UsuarioListItemDto>();
    public IReadOnlyList<RoleOptionViewModel> RolesDisponibles { get; set; } = Array.Empty<RoleOptionViewModel>();
    public int TotalActivos => Usuarios.Count(u => u.Activo);
    public int TotalInactivos => Usuarios.Count(u => !u.Activo);
}

public sealed class UsuarioCreateViewModel
{
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(180, ErrorMessage = "Los nombres no pueden superar los 180 caracteres.")]
    [Display(Name = "Nombres completos")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    [StringLength(254, ErrorMessage = "El correo no puede superar los 254 caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 128 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la contraseña.")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Cuenta activa")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Roles asignados")]
    public List<long> RoleIds { get; set; } = new();

    [StringLength(35, ErrorMessage = "El teléfono no puede superar los 35 caracteres.")]
    [Display(Name = "Teléfono del vocal (opcional)")]
    public string? TelefonoVocal { get; set; }

    public IReadOnlyList<RoleOptionViewModel> RolesDisponibles { get; set; } = Array.Empty<RoleOptionViewModel>();
}

public sealed class UsuarioEditViewModel
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(180, ErrorMessage = "Los nombres no pueden superar los 180 caracteres.")]
    [Display(Name = "Nombres completos")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    [StringLength(254, ErrorMessage = "El correo no puede superar los 254 caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    public bool Activo { get; set; }
    public bool TienePerfilVocal { get; set; }
    public bool VocalActivo { get; set; }

    [Display(Name = "Roles asignados")]
    public List<long> RoleIds { get; set; } = new();

    [StringLength(35, ErrorMessage = "El teléfono no puede superar los 35 caracteres.")]
    [Display(Name = "Teléfono del vocal (opcional)")]
    public string? TelefonoVocal { get; set; }

    public IReadOnlyList<RoleOptionViewModel> RolesDisponibles { get; set; } = Array.Empty<RoleOptionViewModel>();
}

public sealed class UsuarioResetPasswordViewModel
{
    public long UsuarioId { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 128 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la nueva contraseña.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
