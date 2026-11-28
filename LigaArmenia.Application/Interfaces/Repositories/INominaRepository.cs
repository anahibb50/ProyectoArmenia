using LigaArmenia.Domain.Entities;

namespace LigaArmenia.Application.Interfaces.Repositories;

public interface INominaRepository
{
    Task<Equipo?> GetEquipoAsync(long equipoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<long, int>> ContarNominaActivaAsync(long temporadaId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JugadoresEquipo>> ListMembresiasAsync(long equipoId, CancellationToken cancellationToken = default);
    Task<Jugadore?> GetJugadorPorDocumentoAsync(string documento, CancellationToken cancellationToken = default);
    Task<JugadoresEquipo?> GetMembresiaAbiertaEnTemporadaAsync(long jugadorId, long temporadaId, CancellationToken cancellationToken = default);
    Task<bool> ExisteAltaEnFechaAsync(long jugadorId, long equipoId, DateOnly fechaAlta, CancellationToken cancellationToken = default);
    Task<JugadoresEquipo?> GetMembresiaAsync(long membresiaId, CancellationToken cancellationToken = default);
    Task AddJugadorAsync(Jugadore jugador, CancellationToken cancellationToken = default);
    Task AddMembresiaAsync(JugadoresEquipo membresia, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
