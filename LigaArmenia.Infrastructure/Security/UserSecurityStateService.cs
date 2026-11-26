using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LigaArmenia.Infrastructure.Security;

public sealed class UserSecurityStateService : IUserSecurityStateService
{
    private const string CacheKeyPrefix = "sec:user-state:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

    private readonly LigaArmeniaDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public UserSecurityStateService(LigaArmeniaDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<UserSecurityState?> GetStateAsync(long usuarioId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(usuarioId);
        if (_cache.TryGetValue(cacheKey, out UserSecurityState? cached))
            return cached;

        var userRow = await _dbContext.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == usuarioId)
            .Select(u => new
            {
                u.Id,
                u.Nombres,
                u.Correo,
                u.Activo,
                VocalId = u.VocaleUsuario != null && u.VocaleUsuario.Activo ? (long?)u.VocaleUsuario.Id : null,
                Roles = u.UsuariosRoleUsuarios
                    .Select(ur => ur.Rol.Nombre)
                    .ToList(),
                Permissions = u.UsuariosRoleUsuarios
                    .SelectMany(ur => ur.Rol.RolesPermisos)
                    .Select(rp => rp.Permiso.Codigo)
                    .Distinct()
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userRow is null)
            return null;

        var rolesSet = new HashSet<string>(userRow.Roles, StringComparer.OrdinalIgnoreCase);
        var permissionsSet = new HashSet<string>(userRow.Permissions, StringComparer.OrdinalIgnoreCase);

        // El rol ADMINISTRADOR siempre dispone del acceso completo del sistema para evitar bloqueos administrativos.
        if (rolesSet.Contains(SystemRoles.Administrador))
        {
            var allPermissionCodes = await _dbContext.Permisos
                .AsNoTracking()
                .Select(p => p.Codigo)
                .ToListAsync(cancellationToken);

            foreach (var code in allPermissionCodes)
                permissionsSet.Add(code);
        }

        var state = new UserSecurityState(
            userRow.Id,
            userRow.Nombres,
            userRow.Correo,
            userRow.Activo,
            rolesSet,
            permissionsSet,
            userRow.VocalId);

        _cache.Set(cacheKey, state, CacheDuration);
        return state;
    }

    public void InvalidateUser(long usuarioId)
    {
        _cache.Remove(GetCacheKey(usuarioId));
    }

    public void InvalidateUsers(IEnumerable<long> usuarioIds)
    {
        foreach (var id in usuarioIds.Distinct())
            _cache.Remove(GetCacheKey(id));
    }

    private static string GetCacheKey(long usuarioId) => $"{CacheKeyPrefix}{usuarioId}";
}
