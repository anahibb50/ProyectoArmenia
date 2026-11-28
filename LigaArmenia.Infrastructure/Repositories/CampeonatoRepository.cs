using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Repositories;

public sealed class CampeonatoRepository : ICampeonatoRepository
{
    private readonly LigaArmeniaDbContext _dbContext;

    public CampeonatoRepository(LigaArmeniaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Temporada?> GetTemporadaConCategoriasAsync(string nombre, CancellationToken cancellationToken = default)
    {
        return _dbContext.Temporadas
            .Include(t => t.Categorias)
                .ThenInclude(c => c.Etapas)
            .FirstOrDefaultAsync(t => t.Nombre == nombre, cancellationToken);
    }

    public async Task<IReadOnlyList<Temporada>> ListTemporadasAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Temporadas
            .AsNoTracking()
            .OrderBy(t => t.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task MarcarActivaAsync(long temporadaId, long? usuarioId, CancellationToken cancellationToken = default)
    {
        var temporadas = await _dbContext.Temporadas.ToListAsync(cancellationToken);
        var ahora = DateTime.UtcNow;
        foreach (var temporada in temporadas)
        {
            var activa = temporada.Id == temporadaId;
            if (temporada.Activa == activa)
                continue;

            temporada.Activa = activa;
            temporada.ActualizadoEn = ahora;
            temporada.ActualizadoPor = usuarioId;
        }
    }

    public Task AddTemporadaAsync(Temporada temporada, CancellationToken cancellationToken = default)
    {
        return _dbContext.Temporadas.AddAsync(temporada, cancellationToken).AsTask();
    }

    public Task<int> CountEquiposAsync(short categoriaId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Equipos.CountAsync(e => e.CategoriaId == categoriaId, cancellationToken);
    }

    public async Task<IReadOnlyList<Equipo>> ListEquiposAsync(long temporadaId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Equipos
            .AsNoTracking()
            .Where(e => e.TemporadaId == temporadaId)
            .OrderBy(e => e.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExisteNombreEnCategoriaAsync(
        long temporadaId,
        short categoriaId,
        string nombre,
        CancellationToken cancellationToken = default)
    {
        var normalizado = nombre.Trim().ToLower();
        return _dbContext.Equipos.AnyAsync(
            e => e.TemporadaId == temporadaId
                && e.CategoriaId == categoriaId
                && e.Nombre.ToLower() == normalizado,
            cancellationToken);
    }

    public async Task<IReadOnlyList<short>> NumerosDeEtapaConPartidosAsync(
        short categoriaId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Partidos
            .AsNoTracking()
            .Where(p => p.CategoriaId == categoriaId)
            .Select(p => p.Etapa)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public Task AddEquipoAsync(Equipo equipo, CancellationToken cancellationToken = default)
    {
        return _dbContext.Equipos.AddAsync(equipo, cancellationToken).AsTask();
    }

    public void MarcarEtapasParaEliminar(IEnumerable<EtapaCategoria> etapas)
    {
        _dbContext.EtapasCategoria.RemoveRange(etapas);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
