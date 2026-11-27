using LigaArmenia.Application.DTOs.Campeonato;
using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.Interfaces.Persistence;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Domain.Rules;

namespace LigaArmenia.Application.Services;

public sealed class CampeonatoService : ICampeonatoService
{
    private readonly ICampeonatoRepository _repository;
    private readonly ITransactionRunner _transactionRunner;
    private readonly ICurrentUserService _currentUser;

    public CampeonatoService(
        ICampeonatoRepository repository,
        ITransactionRunner transactionRunner,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _transactionRunner = transactionRunner;
        _currentUser = currentUser;
    }

    public Task<OperationResult<TemporadaConfiguradaDto>> AsegurarTemporadaInicialAsync(
        CancellationToken cancellationToken = default)
    {
        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var temporada = await AsegurarTemporadaCoreAsync(ct);
            return OperationResult<TemporadaConfiguradaDto>.Ok(
                MapTemporada(temporada),
                "La temporada 2026-2027 quedó configurada.");
        }, cancellationToken);
    }

    public Task<OperationResult<EquipoCreadoDto>> RegistrarEquipoAsync(
        string codigoCategoria,
        string nombre,
        CancellationToken cancellationToken = default)
    {
        var nombreLimpio = (nombre ?? string.Empty).Trim();
        if (nombreLimpio.Length == 0)
            return Task.FromResult(OperationResult<EquipoCreadoDto>.Fail("El nombre del equipo es obligatorio."));

        if (nombreLimpio.Length > CatalogoTemporadaInicial.MaximoNombreEquipo)
        {
            return Task.FromResult(OperationResult<EquipoCreadoDto>.Fail(
                $"El nombre del equipo no puede superar los {CatalogoTemporadaInicial.MaximoNombreEquipo} caracteres."));
        }

        var definicion = CatalogoTemporadaInicial.Buscar(codigoCategoria);
        if (definicion is null)
            return Task.FromResult(OperationResult<EquipoCreadoDto>.Fail(MensajeCategoriaDesconocida()));

        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var temporada = await AsegurarTemporadaCoreAsync(ct);
            var categoria = BuscarCategoria(temporada, definicion.Codigo);
            if (categoria is null)
                return OperationResult<EquipoCreadoDto>.Fail(MensajeCategoriaDesconocida());

            if (categoria.CupoEquipos is short cupo)
            {
                var registrados = await _repository.CountEquiposAsync(categoria.Id, ct);
                if (registrados >= cupo)
                {
                    return OperationResult<EquipoCreadoDto>.Fail(
                        $"{categoria.Nombre} ya tiene {cupo} equipos, que es el cupo de la categoría.");
                }
            }

            if (await _repository.ExisteNombreEnCategoriaAsync(temporada.Id, categoria.Id, nombreLimpio, ct))
            {
                return OperationResult<EquipoCreadoDto>.Fail(
                    "Ya existe un equipo con ese nombre en esta categoría.");
            }

            var ahora = DateTime.UtcNow;
            var equipo = new Equipo
            {
                TemporadaId = temporada.Id,
                CategoriaId = categoria.Id,
                Nombre = nombreLimpio,
                Serie = categoria.Serie,
                Activo = true,
                CreadoEn = ahora,
                ActualizadoEn = ahora,
                CreadoPor = _currentUser.UserId,
                ActualizadoPor = _currentUser.UserId
            };

            await _repository.AddEquipoAsync(equipo, ct);
            await _repository.SaveChangesAsync(ct);

            return OperationResult<EquipoCreadoDto>.Ok(new EquipoCreadoDto
            {
                Id = equipo.Id,
                TemporadaId = equipo.TemporadaId,
                CategoriaId = equipo.CategoriaId,
                CodigoCategoria = categoria.Codigo,
                Nombre = equipo.Nombre,
                Serie = equipo.Serie
            }, "Equipo registrado.");
        }, cancellationToken);
    }

    public Task<OperationResult<CategoriaDto>> ConfigurarDivisionAsync(
        string codigoCategoria,
        short cupoEquipos,
        IReadOnlyList<EtapaSolicitudDto> etapas,
        CancellationToken cancellationToken = default)
    {
        var definicion = CatalogoTemporadaInicial.Buscar(codigoCategoria);
        if (definicion is null)
            return Task.FromResult(OperationResult<CategoriaDto>.Fail(MensajeCategoriaDesconocida()));

        if (!definicion.EsConfigurable)
        {
            return Task.FromResult(OperationResult<CategoriaDto>.Fail(
                "El cupo y las etapas de Sénior A y Sénior B permanecen fijos."));
        }

        var etapasNormalizadas = NormalizarEtapas(etapas);
        var errores = CatalogoTemporadaInicial.ValidarConfiguracionDivision(cupoEquipos, etapasNormalizadas);
        if (errores.Count > 0)
            return Task.FromResult(OperationResult<CategoriaDto>.Fail(errores));

        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var temporada = await AsegurarTemporadaCoreAsync(ct);
            var categoria = BuscarCategoria(temporada, definicion.Codigo);
            if (categoria is null)
                return OperationResult<CategoriaDto>.Fail(MensajeCategoriaDesconocida());

            var registrados = await _repository.CountEquiposAsync(categoria.Id, ct);
            if (cupoEquipos < registrados)
            {
                return OperationResult<CategoriaDto>.Fail(
                    $"El cupo debe cubrir los {registrados} equipos ya registrados en {categoria.Nombre}.");
            }

            var conflicto = await SincronizarEtapasAsync(categoria, etapasNormalizadas, conservarEtapasConPartidos: false, ct);
            if (conflicto is not null)
                return OperationResult<CategoriaDto>.Fail(conflicto);

            categoria.CupoEquipos = cupoEquipos;
            await _repository.SaveChangesAsync(ct);

            return OperationResult<CategoriaDto>.Ok(
                MapCategoria(categoria),
                $"{categoria.Nombre} quedó configurada.");
        }, cancellationToken);
    }

    public Task<OperationResult> ValidarEtapaDePartidoAsync(
        string codigoCategoria,
        short numeroEtapa,
        CancellationToken cancellationToken = default)
    {
        var definicion = CatalogoTemporadaInicial.Buscar(codigoCategoria);
        if (definicion is null)
            return Task.FromResult(OperationResult.Fail(MensajeCategoriaDesconocida()));

        if (numeroEtapa is < 1 or > CatalogoTemporadaInicial.MaximoEtapas)
        {
            return Task.FromResult(OperationResult.Fail(
                $"La etapa debe estar entre 1 y {CatalogoTemporadaInicial.MaximoEtapas}."));
        }

        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var temporada = await AsegurarTemporadaCoreAsync(ct);
            var categoria = BuscarCategoria(temporada, definicion.Codigo);
            if (categoria is null)
                return OperationResult.Fail(MensajeCategoriaDesconocida());

            if (categoria.Etapas.Any(e => e.Numero == numeroEtapa))
                return OperationResult.Ok();

            return OperationResult.Fail(
                $"{categoria.Nombre} no tiene configurada la etapa {numeroEtapa}.");
        }, cancellationToken);
    }

    private async Task<Temporada> AsegurarTemporadaCoreAsync(CancellationToken cancellationToken)
    {
        var ahora = DateTime.UtcNow;
        var temporada = await _repository.GetTemporadaConCategoriasAsync(
            CatalogoTemporadaInicial.NombreTemporada,
            cancellationToken);

        if (temporada is null)
        {
            temporada = new Temporada
            {
                Nombre = CatalogoTemporadaInicial.NombreTemporada,
                Activa = true,
                CreadoEn = ahora,
                ActualizadoEn = ahora,
                CreadoPor = _currentUser.UserId,
                ActualizadoPor = _currentUser.UserId
            };

            foreach (var definicion in CatalogoTemporadaInicial.Categorias)
                temporada.Categorias.Add(CrearCategoria(definicion));

            await _repository.AddTemporadaAsync(temporada, cancellationToken);
        }
        else
        {
            temporada.ActualizadoEn = ahora;
            temporada.ActualizadoPor = _currentUser.UserId;

            foreach (var definicion in CatalogoTemporadaInicial.Categorias)
            {
                var categoria = BuscarCategoria(temporada, definicion.Codigo);
                if (categoria is null)
                {
                    temporada.Categorias.Add(CrearCategoria(definicion));
                    continue;
                }

                if (definicion.EsConfigurable)
                    continue;

                categoria.Nombre = definicion.Nombre;
                categoria.Disciplina = definicion.Disciplina;
                categoria.Serie = definicion.Serie;
                categoria.CupoEquipos = definicion.CupoEquipos;
                categoria.PuntosVictoria = definicion.PuntosVictoria;
                categoria.PuntosEmpate = definicion.PuntosEmpate;
                categoria.PuntosDerrota = definicion.PuntosDerrota;
                await SincronizarEtapasAsync(categoria, definicion.Etapas, conservarEtapasConPartidos: true, cancellationToken);
            }
        }

        await _repository.SaveChangesAsync(cancellationToken);
        return temporada;
    }

    private async Task<string?> SincronizarEtapasAsync(
        Categoria categoria,
        IReadOnlyList<DefinicionEtapa> deseadas,
        bool conservarEtapasConPartidos,
        CancellationToken cancellationToken)
    {
        var numeros = deseadas.Select(e => e.Numero).ToHashSet();
        var sobrantes = categoria.Etapas.Where(e => !numeros.Contains(e.Numero)).ToList();

        if (sobrantes.Count > 0 && categoria.Id != 0)
        {
            var enUso = await _repository.NumerosDeEtapaConPartidosAsync(categoria.Id, cancellationToken);
            var bloqueadas = sobrantes.Where(e => enUso.Contains(e.Numero)).Select(e => e.Numero).ToList();
            if (bloqueadas.Count > 0)
            {
                if (!conservarEtapasConPartidos)
                {
                    return $"La etapa {bloqueadas[0]} de {categoria.Nombre} ya tiene partidos. Inclúyela en la configuración.";
                }

                sobrantes = sobrantes.Where(e => !enUso.Contains(e.Numero)).ToList();
            }
        }

        if (sobrantes.Count > 0)
        {
            foreach (var sobrante in sobrantes)
                categoria.Etapas.Remove(sobrante);

            _repository.MarcarEtapasParaEliminar(sobrantes);
        }

        foreach (var etapaDef in deseadas)
        {
            var etapa = categoria.Etapas.FirstOrDefault(e => e.Numero == etapaDef.Numero);
            if (etapa is null)
            {
                categoria.Etapas.Add(new EtapaCategoria
                {
                    Numero = etapaDef.Numero,
                    Nombre = etapaDef.Nombre,
                    Formato = etapaDef.Formato
                });
            }
            else
            {
                etapa.Nombre = etapaDef.Nombre;
                etapa.Formato = etapaDef.Formato;
            }
        }

        return null;
    }

    private static Categoria CrearCategoria(DefinicionCategoria definicion)
    {
        var categoria = new Categoria
        {
            Codigo = definicion.Codigo,
            Nombre = definicion.Nombre,
            Disciplina = definicion.Disciplina,
            Serie = definicion.Serie,
            CupoEquipos = definicion.CupoEquipos,
            PuntosVictoria = definicion.PuntosVictoria,
            PuntosEmpate = definicion.PuntosEmpate,
            PuntosDerrota = definicion.PuntosDerrota
        };

        foreach (var etapa in definicion.Etapas)
        {
            categoria.Etapas.Add(new EtapaCategoria
            {
                Numero = etapa.Numero,
                Nombre = etapa.Nombre,
                Formato = etapa.Formato
            });
        }

        return categoria;
    }

    private static Categoria? BuscarCategoria(Temporada temporada, string codigo)
    {
        return temporada.Categorias.FirstOrDefault(c =>
            string.Equals(c.Codigo, codigo, StringComparison.OrdinalIgnoreCase));
    }

    private static List<DefinicionEtapa> NormalizarEtapas(IReadOnlyList<EtapaSolicitudDto>? etapas)
    {
        if (etapas is null || etapas.Count == 0)
            return [];

        return etapas
            .Select(e => new DefinicionEtapa(
                e.Numero,
                (e.Nombre ?? string.Empty).Trim(),
                CatalogoTemporadaInicial.NormalizarCodigo(e.Formato)))
            .ToList();
    }

    private static TemporadaConfiguradaDto MapTemporada(Temporada temporada)
    {
        var orden = CatalogoTemporadaInicial.Categorias
            .Select((definicion, indice) => (definicion.Codigo, indice))
            .ToDictionary(x => x.Codigo, x => x.indice, StringComparer.OrdinalIgnoreCase);

        var categorias = temporada.Categorias
            .OrderBy(c => orden.TryGetValue(c.Codigo, out var indice) ? indice : int.MaxValue)
            .ThenBy(c => c.Codigo)
            .Select(MapCategoria)
            .ToList();

        return new TemporadaConfiguradaDto
        {
            TemporadaId = temporada.Id,
            Nombre = temporada.Nombre,
            Categorias = categorias
        };
    }

    private static CategoriaDto MapCategoria(Categoria categoria)
    {
        var definicion = CatalogoTemporadaInicial.Buscar(categoria.Codigo);
        return new CategoriaDto
        {
            Id = categoria.Id,
            Codigo = categoria.Codigo,
            Nombre = categoria.Nombre,
            Disciplina = categoria.Disciplina,
            Serie = categoria.Serie,
            CupoEquipos = categoria.CupoEquipos,
            PuntosVictoria = categoria.PuntosVictoria,
            PuntosEmpate = categoria.PuntosEmpate,
            PuntosDerrota = categoria.PuntosDerrota,
            EsConfigurable = definicion?.EsConfigurable == true,
            Etapas = categoria.Etapas
                .OrderBy(e => e.Numero)
                .Select(e => new EtapaCategoriaDto
                {
                    Numero = e.Numero,
                    Nombre = e.Nombre,
                    Formato = e.Formato
                })
                .ToList()
        };
    }

    private static string MensajeCategoriaDesconocida()
    {
        return "La categoría indicada no forma parte de la temporada 2026-2027.";
    }
}
