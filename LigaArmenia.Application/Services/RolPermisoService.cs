using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.DTOs.Roles;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;

namespace LigaArmenia.Application.Services;

public sealed class RolPermisoService : IRolPermisoService
{
    private static readonly Dictionary<string, string> ModuleLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["usuarios"] = "Usuarios",
        ["roles"] = "Roles",
        ["permisos"] = "Permisos",
        ["auditoria"] = "Auditoría",
        ["campeonatos"] = "Campeonatos",
        ["equipos"] = "Equipos",
        ["jugadores"] = "Jugadores",
        ["horarios"] = "Horarios",
        ["partidos"] = "Partidos",
        ["alineaciones"] = "Alineaciones",
        ["vocales"] = "Vocales",
        ["vocalias"] = "Vocalías",
        ["tarjetas"] = "Tarjetas",
        ["sanciones"] = "Sanciones",
        ["posiciones"] = "Tabla de Posiciones",
        ["reportes"] = "Reportes"
    };

    private readonly IRolRepository _rolRepository;
    private readonly IPermisoRepository _permisoRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserSecurityStateService _securityStateService;

    public RolPermisoService(
        IRolRepository rolRepository,
        IPermisoRepository permisoRepository,
        ICurrentUserService currentUser,
        IUserSecurityStateService securityStateService)
    {
        _rolRepository = rolRepository;
        _permisoRepository = permisoRepository;
        _currentUser = currentUser;
        _securityStateService = securityStateService;
    }

    public async Task<IReadOnlyList<RolListItemDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _rolRepository.GetAllWithCountsAsync(cancellationToken);
        var allPermissionsCount = (await _permisoRepository.GetAllCodesAsync(cancellationToken)).Count;

        return roles.Select(r =>
        {
            var isAdmin = string.Equals(r.Nombre, SystemRoles.Administrador, StringComparison.OrdinalIgnoreCase);
            return new RolListItemDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                TotalPermisos = isAdmin && r.RolesPermisos.Count == 0 ? allPermissionsCount : r.RolesPermisos.Count,
                TotalUsuarios = r.UsuariosRoles.Count,
                TotalUsuariosActivos = r.UsuariosRoles.Count(ur => ur.Usuario.Activo),
                EsAdministrador = isAdmin
            };
        }).ToList();
    }

    public async Task<RolDetailDto?> GetRoleDetailAsync(long rolId, CancellationToken cancellationToken = default)
    {
        var role = await _rolRepository.GetByIdWithPermissionsAndUsersAsync(rolId, cancellationToken);
        if (role is null)
            return null;

        var allPermissions = await _permisoRepository.GetAllAsync(cancellationToken);
        var isAdmin = string.Equals(role.Nombre, SystemRoles.Administrador, StringComparison.OrdinalIgnoreCase);
        var assignedIds = role.RolesPermisos.Select(rp => rp.PermisoId).ToHashSet();

        var groups = allPermissions
            .GroupBy(p => ExtractModule(p.Codigo), StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => GetModuleLabel(g.Key))
            .Select(g => new PermisoModuloGroupDto
            {
                Modulo = g.Key,
                EtiquetaModulo = GetModuleLabel(g.Key),
                Permisos = g
                    .OrderBy(p => p.Codigo)
                    .Select(p =>
                    {
                        var isCriticalLocked = isAdmin && SystemPermissions.CriticalAdminPermissions.Contains(p.Codigo);
                        var isAssigned = assignedIds.Contains(p.Id) || isCriticalLocked || (isAdmin && assignedIds.Count == 0);
                        return new PermisoItemDto
                        {
                            Id = p.Id,
                            Codigo = p.Codigo,
                            Descripcion = p.Descripcion,
                            Asignado = isAssigned,
                            EsCriticoBloqueado = isCriticalLocked
                        };
                    })
                    .ToList()
            })
            .ToList();

        var users = role.UsuariosRoles
            .Select(ur => new RolUsuarioItemDto
            {
                Id = ur.Usuario.Id,
                Nombres = ur.Usuario.Nombres,
                Correo = ur.Usuario.Correo,
                Activo = ur.Usuario.Activo
            })
            .OrderByDescending(u => u.Activo)
            .ThenBy(u => u.Nombres)
            .ToList();

        return new RolDetailDto
        {
            Id = role.Id,
            Nombre = role.Nombre,
            Descripcion = role.Descripcion,
            EsAdministrador = isAdmin,
            Modulos = groups,
            Usuarios = users
        };
    }

    public async Task<OperationResult> UpdateRolePermissionsAsync(UpdateRolPermisosDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _rolRepository.GetByIdAsync(dto.RolId, cancellationToken);
        if (role is null)
            return OperationResult.Fail("El rol especificado no existe.");

        var allPermissions = await _permisoRepository.GetAllAsync(cancellationToken);
        var validPermissionIds = allPermissions.Select(p => p.Id).ToHashSet();

        var selectedIds = dto.PermisoIds
            .Where(id => validPermissionIds.Contains(id))
            .Distinct()
            .ToHashSet();

        var isAdmin = string.Equals(role.Nombre, SystemRoles.Administrador, StringComparison.OrdinalIgnoreCase);
        if (isAdmin)
        {
            var criticalIds = allPermissions
                .Where(p => SystemPermissions.CriticalAdminPermissions.Contains(p.Codigo))
                .Select(p => p.Id)
                .ToList();

            var missingCritical = criticalIds.Any(id => !selectedIds.Contains(id));
            if (missingCritical)
            {
                return OperationResult.Fail(
                    "No se pueden retirar los permisos críticos de seguridad del rol ADMINISTRADOR, " +
                    "ya que dejaría al sistema sin un administrador funcional.");
            }
        }

        await _rolRepository.ReplaceRolePermissionsAsync(
            role.Id,
            selectedIds,
            _currentUser.UserId,
            DateTime.UtcNow,
            cancellationToken);

        await _rolRepository.SaveChangesAsync(cancellationToken);

        var affectedUsers = await _rolRepository.GetUserIdsByRoleIdAsync(role.Id, cancellationToken);
        _securityStateService.InvalidateUsers(affectedUsers);

        return OperationResult.Ok($"Permisos del rol {role.Nombre} actualizados correctamente.");
    }

    private static string ExtractModule(string codigo)
    {
        var dotIndex = codigo.IndexOf('.');
        return dotIndex > 0 ? codigo[..dotIndex] : "general";
    }

    private static string GetModuleLabel(string moduleKey)
    {
        return ModuleLabels.TryGetValue(moduleKey, out var label)
            ? label
            : char.ToUpperInvariant(moduleKey[0]) + moduleKey[1..];
    }
}
