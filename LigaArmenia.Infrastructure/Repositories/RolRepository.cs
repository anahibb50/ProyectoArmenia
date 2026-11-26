using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Repositories;

public sealed class RolRepository : IRolRepository
{
    private readonly LigaArmeniaDbContext _dbContext;

    public RolRepository(LigaArmeniaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllWithCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AsNoTracking()
            .Include(r => r.RolesPermisos)
            .Include(r => r.UsuariosRoles)
                .ThenInclude(ur => ur.Usuario)
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Role?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<Role?> GetByIdWithPermissionsAndUsersAsync(long id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles
            .AsNoTracking()
            .Include(r => r.RolesPermisos)
                .ThenInclude(rp => rp.Permiso)
            .Include(r => r.UsuariosRoles)
                .ThenInclude(ur => ur.Usuario)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<Role?> GetByNameAsync(string nombre, CancellationToken cancellationToken = default)
    {
        var normalized = nombre.Trim().ToUpperInvariant();
        return _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Nombre.ToUpper() == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToList();
        return await _dbContext.Roles
            .AsNoTracking()
            .Where(r => distinctIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<long>> GetUserIdsByRoleIdAsync(long roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UsuariosRoles
            .AsNoTracking()
            .Where(ur => ur.RolId == roleId)
            .Select(ur => ur.UsuarioId)
            .ToListAsync(cancellationToken);
    }

    public async Task ReplaceRolePermissionsAsync(long roleId, IEnumerable<long> permisoIds, long? actorUserId, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var targetIds = permisoIds.Distinct().ToHashSet();

        var existing = await _dbContext.RolesPermisos
            .Where(rp => rp.RolId == roleId)
            .ToListAsync(cancellationToken);

        var toRemove = existing.Where(e => !targetIds.Contains(e.PermisoId)).ToList();
        if (toRemove.Count > 0)
            _dbContext.RolesPermisos.RemoveRange(toRemove);

        var existingIds = existing.Select(e => e.PermisoId).ToHashSet();
        var toAdd = targetIds
            .Where(id => !existingIds.Contains(id))
            .Select(permisoId => new RolesPermiso
            {
                RolId = roleId,
                PermisoId = permisoId,
                CreadoEn = utcNow,
                ActualizadoEn = utcNow,
                CreadoPor = actorUserId,
                ActualizadoPor = actorUserId
            })
            .ToList();

        if (toAdd.Count > 0)
            await _dbContext.RolesPermisos.AddRangeAsync(toAdd, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
