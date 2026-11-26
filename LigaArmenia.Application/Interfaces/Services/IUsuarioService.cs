using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.DTOs.Users;

namespace LigaArmenia.Application.Interfaces.Services;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioListItemDto>> SearchAsync(UsuarioFilterDto filter, CancellationToken cancellationToken = default);
    Task<UsuarioDetailDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<OperationResult<long>> CreateAsync(CreateUsuarioDto dto, CancellationToken cancellationToken = default);
    Task<OperationResult> UpdateAsync(UpdateUsuarioDto dto, CancellationToken cancellationToken = default);
    Task<OperationResult> SetActiveStatusAsync(long usuarioId, bool activo, CancellationToken cancellationToken = default);
    Task<OperationResult> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
}
