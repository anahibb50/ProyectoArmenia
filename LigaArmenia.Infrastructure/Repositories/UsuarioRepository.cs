using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly LigaArmeniaDbContext _dbContext;

    public UsuarioRepository(LigaArmeniaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Usuario?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<Usuario?> GetByIdWithRolesAsync(long id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios
            .Include(u => u.UsuariosRoleUsuarios)
                .ThenInclude(ur => ur.Rol)
            .Include(u => u.VocaleUsuario)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<Usuario?> GetByEmailWithRolesAndPermissionsAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios
            .Include(u => u.UsuariosRoleUsuarios)
                .ThenInclude(ur => ur.Rol)
                    .ThenInclude(r => r.RolesPermisos)
                        .ThenInclude(rp => rp.Permiso)
            .Include(u => u.VocaleUsuario)
            .FirstOrDefaultAsync(u => u.Correo.ToLower() == normalizedEmail, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string normalizedEmail, long? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Usuarios.AsNoTracking()
            .Where(u => u.Correo.ToLower() == normalizedEmail);

        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Usuario>> SearchAsync(string? search, long? rolId, bool? activo, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Usuarios
            .AsNoTracking()
            .Include(u => u.UsuariosRoleUsuarios)
                .ThenInclude(ur => ur.Rol)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(u =>
                u.Nombres.ToLower().Contains(term) ||
                u.Correo.ToLower().Contains(term));
        }

        if (rolId.HasValue)
        {
            query = query.Where(u => u.UsuariosRoleUsuarios.Any(ur => ur.RolId == rolId.Value));
        }

        if (activo.HasValue)
        {
            query = query.Where(u => u.Activo == activo.Value);
        }

        return await query
            .OrderByDescending(u => u.Activo)
            .ThenBy(u => u.Nombres)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountActiveUsersInRoleAsync(string roleName, long? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var normalizedRole = roleName.Trim().ToUpperInvariant();
        var query = _dbContext.Usuarios
            .AsNoTracking()
            .Where(u => u.Activo &&
                        u.UsuariosRoleUsuarios.Any(ur => ur.Rol.Nombre.ToUpper() == normalizedRole));

        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);

        return query.CountAsync(cancellationToken);
    }

    public Task<bool> AnyUserInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var normalizedRole = roleName.Trim().ToUpperInvariant();
        return _dbContext.UsuariosRoles
            .AsNoTracking()
            .AnyAsync(ur => ur.Rol.Nombre.ToUpper() == normalizedRole, cancellationToken);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _dbContext.Usuarios.AddAsync(usuario, cancellationToken);
    }

    public async Task ReplaceUserRolesAsync(long usuarioId, IEnumerable<long> roleIds, long? actorUserId, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var targetRoleIds = roleIds.Distinct().ToHashSet();

        var existing = await _dbContext.UsuariosRoles
            .Where(ur => ur.UsuarioId == usuarioId)
            .ToListAsync(cancellationToken);

        var toRemove = existing.Where(e => !targetRoleIds.Contains(e.RolId)).ToList();
        if (toRemove.Count > 0)
            _dbContext.UsuariosRoles.RemoveRange(toRemove);

        var existingRoleIds = existing.Select(e => e.RolId).ToHashSet();
        var toAdd = targetRoleIds
            .Where(id => !existingRoleIds.Contains(id))
            .Select(roleId => new UsuariosRole
            {
                UsuarioId = usuarioId,
                RolId = roleId,
                CreadoEn = utcNow,
                ActualizadoEn = utcNow,
                CreadoPor = actorUserId,
                ActualizadoPor = actorUserId
            })
            .ToList();

        if (toAdd.Count > 0)
            await _dbContext.UsuariosRoles.AddRangeAsync(toAdd, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
