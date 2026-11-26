using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Repositories;

public sealed class PermisoRepository : IPermisoRepository
{
    private readonly LigaArmeniaDbContext _dbContext;

    public PermisoRepository(LigaArmeniaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Permiso>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permisos
            .AsNoTracking()
            .OrderBy(p => p.Codigo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permiso>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToList();
        return await _dbContext.Permisos
            .AsNoTracking()
            .Where(p => distinctIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetAllCodesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permisos
            .AsNoTracking()
            .OrderBy(p => p.Codigo)
            .Select(p => p.Codigo)
            .ToListAsync(cancellationToken);
    }
}
