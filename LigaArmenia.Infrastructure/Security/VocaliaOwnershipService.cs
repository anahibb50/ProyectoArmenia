using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Security;

public sealed class VocaliaOwnershipService : IVocaliaOwnershipService
{
    private readonly LigaArmeniaDbContext _dbContext;
    private readonly IUserSecurityStateService _securityStateService;

    public VocaliaOwnershipService(
        LigaArmeniaDbContext dbContext,
        IUserSecurityStateService securityStateService)
    {
        _dbContext = dbContext;
        _securityStateService = securityStateService;
    }

    public async Task<bool> IsUserAssignedToMatchAsync(long usuarioId, long partidoId, CancellationToken cancellationToken = default)
    {
        var state = await _securityStateService.GetStateAsync(usuarioId, cancellationToken);
        if (state is null || !state.Activo)
            return false;

        if (state.IsAdmin)
            return true;

        if (!state.VocalId.HasValue)
            return false;

        return await _dbContext.DesignacionesVocalia
            .AsNoTracking()
            .AnyAsync(d => d.PartidoId == partidoId && d.VocalId == state.VocalId.Value, cancellationToken);
    }

    public async Task<bool> CanUserAccessVocaliaAsync(long usuarioId, long vocaliaId, CancellationToken cancellationToken = default)
    {
        var state = await _securityStateService.GetStateAsync(usuarioId, cancellationToken);
        if (state is null || !state.Activo)
            return false;

        if (state.IsAdmin)
            return true;

        var vocalia = await _dbContext.Vocaliases
            .AsNoTracking()
            .Where(v => v.Id == vocaliaId)
            .Select(v => new { v.PartidoId, v.CreadoPor })
            .FirstOrDefaultAsync(cancellationToken);

        if (vocalia is null)
            return false;

        if (vocalia.CreadoPor == usuarioId)
            return true;

        if (!state.VocalId.HasValue)
            return false;

        return await _dbContext.DesignacionesVocalia
            .AsNoTracking()
            .AnyAsync(d => d.PartidoId == vocalia.PartidoId && d.VocalId == state.VocalId.Value, cancellationToken);
    }
}
