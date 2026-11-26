using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.DTOs.Vocales;

namespace LigaArmenia.Application.Interfaces.Services;

public interface IVocalService
{
    Task<VocalPagedResultDto> SearchPagedAsync(VocalFilterDto filter, CancellationToken cancellationToken = default);
    Task<VocalDetailDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<OperationResult> UpdateAsync(UpdateVocalDto dto, CancellationToken cancellationToken = default);
    Task<OperationResult> SetActiveStatusAsync(long vocalId, bool activo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna los vocales disponibles para futuras designaciones de partidos
    /// (perfil de vocal activo + usuario activo + rol VOCAL asignado).
    /// </summary>
    Task<IReadOnlyList<VocalDisponibleDto>> GetAvailableForDesignationAsync(CancellationToken cancellationToken = default);
}
