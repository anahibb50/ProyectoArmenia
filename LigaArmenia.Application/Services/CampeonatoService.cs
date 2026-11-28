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

    public Task<OperationResult<PanelCampeonatoDto>> ObtenerPanelAsync(
        string? nombreTemporada,
        CancellationToken cancellationToken = default)
    {
        return _transactionRunner.ExecuteAsync(async ct =>
        {
            await PrepararVigenteAsync(ct);
            var temporada = await ResolverTemporadaAsync(nombreTemporada, crearSiNoExiste: false, ct);
            if (temporada is null)
            {
                return OperationResult<PanelCampeonatoDto>.Fail(
                    "Esa temporada no está abierta. Abre el año siguiente desde el campeonato.");
            }

            return OperationResult<PanelCampeonatoDto>.Ok(await MapPanelAsync(temporada, ct));
        }, cancellationToken);
    }

    public Task<OperationResult<string>> AbrirSiguienteTemporadaAsync(CancellationToken cancellationToken = default)
    {
        return _transactionRunner.ExecuteAsync(async ct =>
        {
            await PrepararVigenteAsync(ct);
            var temporadas = await _repository.ListTemporadasAsync(ct);
            var siguiente = SiguienteNombre(temporadas.Select(t => t.Nombre));
            var (_, creada) = await AsegurarTemporadaCoreAsync(siguiente, activaAlCrear: false, ct);
            var mensaje = creada
                ? $"La temporada {siguiente} quedó abierta. Los equipos que registres ahí pertenecen a ese año."
                : $"La temporada {siguiente} ya estaba abierta.";
            return OperationResult<string>.Ok(siguiente, mensaje);
        }, cancellationToken);
    }

    public Task<OperationResult> MarcarTemporadaActualAsync(
        string nombreTemporada,
        CancellationToken cancellationToken = default)
    {
        if (!CatalogoTemporadaInicial.TryParse(nombreTemporada, out _))
            return Task.FromResult(OperationResult.Fail("La temporada debe tener el formato 2025-2026."));

        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var temporada = await _repository.GetTemporadaConCategoriasAsync(nombreTemporada.Trim(), ct);
            if (temporada is null)
                return OperationResult.Fail("Esa temporada no está abierta.");

            await _repository.MarcarActivaAsync(temporada.Id, _currentUser.UserId, ct);
            await _repository.SaveChangesAsync(ct);
            return OperationResult.Ok($"{temporada.Nombre} es ahora la temporada actual.");
        }, cancellationToken);
    }

    public Task<OperationResult<EquipoCreadoDto>> RegistrarEquipoAsync(
        string nombreTemporada,
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
            var temporada = await ObtenerTemporadaAbiertaAsync(nombreTemporada, ct);
            if (temporada is null)
                return OperationResult<EquipoCreadoDto>.Fail("Selecciona una temporada abierta.");

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
            }, $"Equipo registrado en la temporada {temporada.Nombre}.");
        }, cancellationToken);
    }

    public Task<OperationResult<CategoriaDto>> ConfigurarDivisionAsync(
        string nombreTemporada,
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
            var temporada = await ObtenerTemporadaAbiertaAsync(nombreTemporada, ct);
            if (temporada is null)
                return OperationResult<CategoriaDto>.Fail("Selecciona una temporada abierta.");

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
                $"{categoria.Nombre} quedó configurada en la temporada {temporada.Nombre}.");
        }, cancellationToken);
    }

    public Task<OperationResult> ValidarEtapaDePartidoAsync(
        string nombreTemporada,
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
            var temporada = await ObtenerTemporadaAbiertaAsync(nombreTemporada, ct);
            if (temporada is null)
                return OperationResult.Fail("Selecciona una temporada abierta.");

            var categoria = BuscarCategoria(temporada, definicion.Codigo);
            if (categoria is null)
                return OperationResult.Fail(MensajeCategoriaDesconocida());

            if (categoria.Etapas.Any(e => e.Numero == numeroEtapa))
                return OperationResult.Ok();

            return OperationResult.Fail(
                $"{categoria.Nombre} no tiene configurada la etapa {numeroEtapa}.");
        }, cancellationToken);
    }

    private async Task PrepararVigenteAsync(CancellationToken cancellationToken)
    {
        var (temporada, creada) = await AsegurarTemporadaCoreAsync(
            CatalogoTemporadaInicial.TemporadaVigente,
            activaAlCrear: true,
            cancellationToken);

        if (!creada)
            return;

        await _repository.MarcarActivaAsync(temporada.Id, _currentUser.UserId, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Temporada?> ResolverTemporadaAsync(
        string? nombreTemporada,
        bool crearSiNoExiste,
        CancellationToken cancellationToken)
    {
        var nombre = (nombreTemporada ?? string.Empty).Trim();
        if (!CatalogoTemporadaInicial.TryParse(nombre, out _))
        {
            var activas = await _repository.ListTemporadasAsync(cancellationToken);
            var activa = activas.FirstOrDefault(t => t.Activa)
                ?? activas.FirstOrDefault(t => t.Nombre == CatalogoTemporadaInicial.TemporadaVigente)
                ?? activas.FirstOrDefault();
            nombre = activa?.Nombre ?? CatalogoTemporadaInicial.TemporadaVigente;
        }

        if (crearSiNoExiste)
        {
            var (temporada, _) = await AsegurarTemporadaCoreAsync(nombre, activaAlCrear: false, cancellationToken);
            return temporada;
        }

        return await ObtenerTemporadaAbiertaAsync(nombre, cancellationToken);
    }

    private async Task<Temporada?> ObtenerTemporadaAbiertaAsync(string nombreTemporada, CancellationToken cancellationToken)
    {
        if (!CatalogoTemporadaInicial.TryParse(nombreTemporada, out _))
            return null;

        var temporada = await _repository.GetTemporadaConCategoriasAsync(nombreTemporada.Trim(), cancellationToken);
        if (temporada is null)
            return null;

        await SincronizarDefinicionesAsync(temporada, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return temporada;
    }

    private async Task<(Temporada Temporada, bool Creada)> AsegurarTemporadaCoreAsync(
        string nombre,
        bool activaAlCrear,
        CancellationToken cancellationToken)
    {
        var ahora = DateTime.UtcNow;
        var temporada = await _repository.GetTemporadaConCategoriasAsync(nombre, cancellationToken);
        if (temporada is not null)
        {
            await SincronizarDefinicionesAsync(temporada, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return (temporada, false);
        }

        temporada = new Temporada
        {
            Nombre = nombre,
            Activa = activaAlCrear,
            CreadoEn = ahora,
            ActualizadoEn = ahora,
            CreadoPor = _currentUser.UserId,
            ActualizadoPor = _currentUser.UserId
        };

        foreach (var definicion in CatalogoTemporadaInicial.Categorias)
            temporada.Categorias.Add(CrearCategoria(definicion));

        await _repository.AddTemporadaAsync(temporada, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return (temporada, true);
    }

    private async Task SincronizarDefinicionesAsync(Temporada temporada, CancellationToken cancellationToken)
    {
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

    private async Task<PanelCampeonatoDto> MapPanelAsync(Temporada temporada, CancellationToken cancellationToken)
    {
        var temporadas = await _repository.ListTemporadasAsync(cancellationToken);
        var equipos = await _repository.ListEquiposAsync(temporada.Id, cancellationToken);
        var resumen = temporadas.FirstOrDefault(t => t.Id == temporada.Id);
        var configurada = MapTemporada(temporada);
        if (resumen is not null)
            configurada = new TemporadaConfiguradaDto
            {
                TemporadaId = configurada.TemporadaId,
                Nombre = configurada.Nombre,
                Activa = resumen.Activa,
                Categorias = configurada.Categorias
            };

        return new PanelCampeonatoDto
        {
            Temporada = configurada,
            SiguienteTemporada = SiguienteNombre(temporadas.Select(t => t.Nombre)),
            Temporadas = temporadas.Select(t => new TemporadaResumenDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Activa = t.Activa
            }).ToList(),
            Equipos = equipos.Select(e => new EquipoListadoDto
            {
                Id = e.Id,
                CategoriaId = e.CategoriaId,
                Nombre = e.Nombre,
                Activo = e.Activo
            }).ToList()
        };
    }

    private static string SiguienteNombre(IEnumerable<string> nombres)
    {
        var maximo = 0;
        foreach (var nombre in nombres)
        {
            if (CatalogoTemporadaInicial.TryParse(nombre, out var inicio) && inicio > maximo)
                maximo = inicio;
        }

        if (maximo == 0 && CatalogoTemporadaInicial.TryParse(CatalogoTemporadaInicial.TemporadaVigente, out var vigente))
            maximo = vigente - 1;

        return CatalogoTemporadaInicial.Formatear(maximo + 1);
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
            Activa = temporada.Activa,
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
        return "La categoría indicada no forma parte de esta temporada.";
    }
}
