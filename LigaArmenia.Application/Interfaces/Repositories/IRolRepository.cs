using LigaArmenia.Domain.Entities;

namespace LigaArmenia.Application.Interfaces.Repositories;

public interface IRolRepository
{
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllWithCountsAsync(CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Role?> GetByIdWithPermissionsAndUsersAsync(long id, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string nombre, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<long>> GetUserIdsByRoleIdAsync(long roleId, CancellationToken cancellationToken = default);
    Task ReplaceRolePermissionsAsync(long roleId, IEnumerable<long> permisoIds, long? actorUserId, DateTime utcNow, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
