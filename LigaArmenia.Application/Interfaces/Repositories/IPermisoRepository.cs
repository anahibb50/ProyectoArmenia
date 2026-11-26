using LigaArmenia.Domain.Entities;

namespace LigaArmenia.Application.Interfaces.Repositories;

public interface IPermisoRepository
{
    Task<IReadOnlyList<Permiso>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permiso>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetAllCodesAsync(CancellationToken cancellationToken = default);
}
