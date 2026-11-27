using LigaArmenia.Application.DTOs.Campeonato;
using LigaArmenia.Application.DTOs.Common;

namespace LigaArmenia.Application.Interfaces.Services;

public interface ICampeonatoService
{
    Task<OperationResult<TemporadaConfiguradaDto>> AsegurarTemporadaInicialAsync(CancellationToken cancellationToken = default);

    Task<OperationResult<EquipoCreadoDto>> RegistrarEquipoAsync(
        string codigoCategoria,
        string nombre,
        CancellationToken cancellationToken = default);

    Task<OperationResult<CategoriaDto>> ConfigurarDivisionAsync(
        string codigoCategoria,
        short cupoEquipos,
        IReadOnlyList<EtapaSolicitudDto> etapas,
        CancellationToken cancellationToken = default);

    Task<OperationResult> ValidarEtapaDePartidoAsync(
        string codigoCategoria,
        short numeroEtapa,
        CancellationToken cancellationToken = default);
}
