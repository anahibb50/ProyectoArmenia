using LigaArmenia.Domain.Entities;

namespace LigaArmenia.Application.Interfaces.Repositories;

public interface ICampeonatoRepository
{
    Task<Temporada?> GetTemporadaConCategoriasAsync(string nombre, CancellationToken cancellationToken = default);
    Task AddTemporadaAsync(Temporada temporada, CancellationToken cancellationToken = default);
    Task<int> CountEquiposAsync(short categoriaId, CancellationToken cancellationToken = default);
    Task<bool> ExisteNombreEnCategoriaAsync(long temporadaId, short categoriaId, string nombre, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<short>> NumerosDeEtapaConPartidosAsync(short categoriaId, CancellationToken cancellationToken = default);
    Task AddEquipoAsync(Equipo equipo, CancellationToken cancellationToken = default);
    void MarcarEtapasParaEliminar(IEnumerable<EtapaCategoria> etapas);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
