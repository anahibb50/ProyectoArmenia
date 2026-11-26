using LigaArmenia.Application.DTOs.Vocales;
using LigaArmenia.Domain.Entities;

namespace LigaArmenia.Application.Interfaces.Repositories;

public interface IVocalRepository
{
    Task<Vocale?> GetByIdWithUserAsync(long id, CancellationToken cancellationToken = default);
    Task<Vocale?> GetByUsuarioIdAsync(long usuarioId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Vocale> Items, int TotalFiltered, int TotalRegistrados, int TotalActivos, int TotalInactivos)> SearchPagedAsync(
        string? busqueda,
        bool? activo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VocalDisponibleDto>> GetAvailableForDesignationAsync(CancellationToken cancellationToken = default);
    Task<int> CountPendingAssignedMatchesAsync(long vocalId, CancellationToken cancellationToken = default);
    Task AddAsync(Vocale vocal, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
