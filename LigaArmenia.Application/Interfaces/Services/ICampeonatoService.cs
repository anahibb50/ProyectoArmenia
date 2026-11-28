using LigaArmenia.Application.DTOs.Campeonato;
using LigaArmenia.Application.DTOs.Common;

namespace LigaArmenia.Application.Interfaces.Services;

public interface ICampeonatoService
{
    Task<OperationResult<PanelCampeonatoDto>> ObtenerPanelAsync(
        string? nombreTemporada,
        CancellationToken cancellationToken = default);

    Task<OperationResult<EquipoCreadoDto>> RegistrarEquipoAsync(
        string nombreTemporada,
        string codigoCategoria,
        string nombre,
        CancellationToken cancellationToken = default);

    Task<OperationResult<CategoriaDto>> ConfigurarDivisionAsync(
        string nombreTemporada,
        string codigoCategoria,
        short cupoEquipos,
        IReadOnlyList<EtapaSolicitudDto> etapas,
        CancellationToken cancellationToken = default);

    Task<OperationResult<string>> AbrirSiguienteTemporadaAsync(CancellationToken cancellationToken = default);

    Task<OperationResult> MarcarTemporadaActualAsync(
        string nombreTemporada,
        CancellationToken cancellationToken = default);

    Task<OperationResult> ValidarEtapaDePartidoAsync(
        string nombreTemporada,
        string codigoCategoria,
        short numeroEtapa,
        CancellationToken cancellationToken = default);
}
