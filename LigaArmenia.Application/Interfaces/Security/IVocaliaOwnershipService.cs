namespace LigaArmenia.Application.Interfaces.Security;

/// <summary>
/// Servicio reutilizable para validar si un usuario (vocal) tiene asignado un partido
/// o es propietario de una vocalía registrada.
/// </summary>
public interface IVocaliaOwnershipService
{
    Task<bool> IsUserAssignedToMatchAsync(long usuarioId, long partidoId, CancellationToken cancellationToken = default);
    Task<bool> CanUserAccessVocaliaAsync(long usuarioId, long vocaliaId, CancellationToken cancellationToken = default);
}
