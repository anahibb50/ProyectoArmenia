using LigaArmenia.Domain.Entities;

namespace LigaArmenia.Application.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByIdWithRolesAsync(long id, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByEmailWithRolesAndPermissionsAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string normalizedEmail, long? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Usuario>> SearchAsync(string? search, long? rolId, bool? activo, CancellationToken cancellationToken = default);
    Task<int> CountActiveUsersInRoleAsync(string roleName, long? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<bool> AnyUserInRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task ReplaceUserRolesAsync(long usuarioId, IEnumerable<long> roleIds, long? actorUserId, DateTime utcNow, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
