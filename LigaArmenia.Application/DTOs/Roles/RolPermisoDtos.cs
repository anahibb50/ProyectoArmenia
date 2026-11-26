namespace LigaArmenia.Application.DTOs.Roles;

public sealed class RolListItemDto
{
    public long Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public int TotalPermisos { get; init; }
    public int TotalUsuarios { get; init; }
    public int TotalUsuariosActivos { get; init; }
    public bool EsAdministrador { get; init; }
}

public sealed class PermisoItemDto
{
    public long Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public bool Asignado { get; init; }
    public bool EsCriticoBloqueado { get; init; }
}

public sealed class PermisoModuloGroupDto
{
    public string Modulo { get; init; } = string.Empty;
    public string EtiquetaModulo { get; init; } = string.Empty;
    public IReadOnlyList<PermisoItemDto> Permisos { get; init; } = Array.Empty<PermisoItemDto>();
}

public sealed class RolUsuarioItemDto
{
    public long Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public bool Activo { get; init; }
}

public sealed class RolDetailDto
{
    public long Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public bool EsAdministrador { get; init; }
    public IReadOnlyList<PermisoModuloGroupDto> Modulos { get; init; } = Array.Empty<PermisoModuloGroupDto>();
    public IReadOnlyList<RolUsuarioItemDto> Usuarios { get; init; } = Array.Empty<RolUsuarioItemDto>();
}

public sealed class UpdateRolPermisosDto
{
    public long RolId { get; set; }
    public List<long> PermisoIds { get; set; } = new();
}
