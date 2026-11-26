using LigaArmenia.Application.DTOs.Vocales;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Repositories;

public sealed class VocalRepository : IVocalRepository
{
    private static readonly string[] ClosedMatchStates =
    {
        "FINALIZADO",
        "APROBADO",
        "CANCELADO",
        "ANULADO",
        "SUSPENDIDO_DEFINITIVO"
    };

    private readonly LigaArmeniaDbContext _dbContext;

    public VocalRepository(LigaArmeniaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Vocale?> GetByIdWithUserAsync(long id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Vocales
            .Include(v => v.Usuario)
                .ThenInclude(u => u!.UsuariosRoleUsuarios)
                    .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public Task<Vocale?> GetByUsuarioIdAsync(long usuarioId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Vocales
            .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId, cancellationToken);
    }

    public async Task<(IReadOnlyList<Vocale> Items, int TotalFiltered, int TotalRegistrados, int TotalActivos, int TotalInactivos)> SearchPagedAsync(
        string? busqueda,
        bool? activo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalRegistrados = await _dbContext.Vocales.AsNoTracking().CountAsync(cancellationToken);
        var totalActivos = await _dbContext.Vocales.AsNoTracking().CountAsync(v => v.Activo, cancellationToken);
        var totalInactivos = totalRegistrados - totalActivos;

        var query = _dbContext.Vocales
            .AsNoTracking()
            .Include(v => v.Usuario)
                .ThenInclude(u => u!.UsuariosRoleUsuarios)
                    .ThenInclude(ur => ur.Rol)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var term = busqueda.Trim().ToLowerInvariant();
            query = query.Where(v =>
                v.Nombres.ToLower().Contains(term) ||
                (v.Telefono != null && v.Telefono.ToLower().Contains(term)) ||
                (v.Usuario != null && v.Usuario.Correo.ToLower().Contains(term)));
        }

        if (activo.HasValue)
        {
            query = query.Where(v => v.Activo == activo.Value);
        }

        var totalFiltered = await query.CountAsync(cancellationToken);

        var safePageSize = Math.Clamp(pageSize, 5, 50);
        var totalPages = Math.Max(1, (int)Math.Ceiling((double)totalFiltered / safePageSize));
        var safePage = Math.Clamp(page, 1, totalPages);

        var items = await query
            .OrderByDescending(v => v.Activo)
            .ThenBy(v => v.Nombres)
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        return (items, totalFiltered, totalRegistrados, totalActivos, totalInactivos);
    }

    public async Task<IReadOnlyList<VocalDisponibleDto>> GetAvailableForDesignationAsync(CancellationToken cancellationToken = default)
    {
        var vocalRole = SystemRoles.Vocal.ToUpperInvariant();

        return await _dbContext.Vocales
            .AsNoTracking()
            .Where(v =>
                v.Activo &&
                v.Usuario != null &&
                v.Usuario.Activo &&
                v.Usuario.UsuariosRoleUsuarios.Any(ur => ur.Rol.Nombre.ToUpper() == vocalRole))
            .OrderBy(v => v.Nombres)
            .Select(v => new VocalDisponibleDto
            {
                Id = v.Id,
                Nombres = v.Nombres
            })
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountPendingAssignedMatchesAsync(long vocalId, CancellationToken cancellationToken = default)
    {
        return _dbContext.DesignacionesVocalia
            .AsNoTracking()
            .Where(d =>
                d.VocalId == vocalId &&
                d.Partido.AprobadoEn == null &&
                !ClosedMatchStates.Contains(d.Partido.Estado.ToUpper()))
            .CountAsync(cancellationToken);
    }

    public async Task AddAsync(Vocale vocal, CancellationToken cancellationToken = default)
    {
        await _dbContext.Vocales.AddAsync(vocal, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
