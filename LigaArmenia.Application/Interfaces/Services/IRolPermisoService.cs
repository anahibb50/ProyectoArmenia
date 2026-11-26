using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.DTOs.Roles;

namespace LigaArmenia.Application.Interfaces.Services;

public interface IRolPermisoService
{
    Task<IReadOnlyList<RolListItemDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<RolDetailDto?> GetRoleDetailAsync(long rolId, CancellationToken cancellationToken = default);
    Task<OperationResult> UpdateRolePermissionsAsync(UpdateRolPermisosDto dto, CancellationToken cancellationToken = default);
}
