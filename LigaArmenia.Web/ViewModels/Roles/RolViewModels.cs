using LigaArmenia.Application.DTOs.Roles;

namespace LigaArmenia.Web.ViewModels.Roles;

public sealed class RolIndexViewModel
{
    public IReadOnlyList<RolListItemDto> Roles { get; init; } = Array.Empty<RolListItemDto>();
}

public sealed class RolManagePermissionsViewModel
{
    public long RolId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool EsAdministrador { get; set; }
    public List<long> SelectedPermissionIds { get; set; } = new();
    public IReadOnlyList<PermisoModuloGroupDto> Modulos { get; set; } = Array.Empty<PermisoModuloGroupDto>();
    public IReadOnlyList<RolUsuarioItemDto> Usuarios { get; set; } = Array.Empty<RolUsuarioItemDto>();
}
