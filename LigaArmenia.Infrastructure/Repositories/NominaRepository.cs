using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Repositories;

public sealed class NominaRepository : INominaRepository
{
    private readonly LigaArmeniaDbContext _dbContext;

    public NominaRepository(LigaArmeniaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Equipo?> GetEquipoAsync(long equipoId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Equipos
            .Include(e => e.Categoria)
            .Include(e => e.Temporada)
            .FirstOrDefaultAsync(e => e.Id == equipoId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<long, int>> ContarNominaActivaAsync(
        long temporadaId,
        CancellationToken cancellationToken = default)
    {
        var filas = await _dbContext.JugadoresEquipos
            .AsNoTracking()
            .Where(m => m.FechaBaja == null && m.Equipo.TemporadaId == temporadaId)
            .GroupBy(m => m.EquipoId)
            .Select(g => new { EquipoId = g.Key, Total = g.Count() })
            .ToListAsync(cancellationToken);

        return filas.ToDictionary(f => f.EquipoId, f => f.Total);
    }

    public async Task<IReadOnlyList<JugadoresEquipo>> ListMembresiasAsync(
        long equipoId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.JugadoresEquipos
            .AsNoTracking()
            .Include(m => m.Jugador)
            .Where(m => m.EquipoId == equipoId)
            .OrderBy(m => m.FechaBaja != null)
            .ThenBy(m => m.Jugador.Nombres)
            .ToListAsync(cancellationToken);
    }

    public Task<Jugadore?> GetJugadorPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        var normalizado = documento.ToLower();
        return _dbContext.Jugadores
            .FirstOrDefaultAsync(j => j.Documento.ToLower() == normalizado, cancellationToken);
    }

    public Task<JugadoresEquipo?> GetMembresiaAbiertaEnTemporadaAsync(
        long jugadorId,
        long temporadaId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.JugadoresEquipos
            .Include(m => m.Equipo)
            .FirstOrDefaultAsync(
                m => m.JugadorId == jugadorId
                    && m.FechaBaja == null
                    && m.Equipo.TemporadaId == temporadaId,
                cancellationToken);
    }

    public Task<bool> ExisteAltaEnFechaAsync(
        long jugadorId,
        long equipoId,
        DateOnly fechaAlta,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.JugadoresEquipos.AnyAsync(
            m => m.JugadorId == jugadorId && m.EquipoId == equipoId && m.FechaAlta == fechaAlta,
            cancellationToken);
    }

    public Task<JugadoresEquipo?> GetMembresiaAsync(long membresiaId, CancellationToken cancellationToken = default)
    {
        return _dbContext.JugadoresEquipos
            .Include(m => m.Jugador)
            .Include(m => m.Equipo)
            .FirstOrDefaultAsync(m => m.Id == membresiaId, cancellationToken);
    }

    public Task AddJugadorAsync(Jugadore jugador, CancellationToken cancellationToken = default)
    {
        return _dbContext.Jugadores.AddAsync(jugador, cancellationToken).AsTask();
    }

    public Task AddMembresiaAsync(JugadoresEquipo membresia, CancellationToken cancellationToken = default)
    {
        return _dbContext.JugadoresEquipos.AddAsync(membresia, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
