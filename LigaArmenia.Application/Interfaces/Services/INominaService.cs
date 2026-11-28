using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.DTOs.Nomina;

namespace LigaArmenia.Application.Interfaces.Services;

public interface INominaService
{
    Task<IReadOnlyDictionary<long, int>> ContarNominaActivaAsync(long temporadaId, CancellationToken cancellationToken = default);

    Task<OperationResult<NominaEquipoDto>> ObtenerNominaAsync(long equipoId, CancellationToken cancellationToken = default);

    Task<OperationResult<JugadorNominaDto>> AgregarJugadorAsync(
        long equipoId,
        string documento,
        string nombres,
        DateOnly? fechaNacimiento,
        bool consideradoJuvenil,
        bool habilitado,
        CancellationToken cancellationToken = default);

    Task<OperationResult> CambiarHabilitacionAsync(long membresiaId, bool habilitado, CancellationToken cancellationToken = default);

    Task<OperationResult> CambiarCondicionJuvenilAsync(long membresiaId, bool consideradoJuvenil, CancellationToken cancellationToken = default);

    Task<OperationResult> DarDeBajaAsync(long membresiaId, CancellationToken cancellationToken = default);
}
