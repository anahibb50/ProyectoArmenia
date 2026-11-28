using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.DTOs.Nomina;
using LigaArmenia.Application.Interfaces.Persistence;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Domain.Rules;

namespace LigaArmenia.Application.Services;

public sealed class NominaService : INominaService
{
    private readonly INominaRepository _repository;
    private readonly ITransactionRunner _transactionRunner;
    private readonly ICurrentUserService _currentUser;

    public NominaService(
        INominaRepository repository,
        ITransactionRunner transactionRunner,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _transactionRunner = transactionRunner;
        _currentUser = currentUser;
    }

    public Task<IReadOnlyDictionary<long, int>> ContarNominaActivaAsync(
        long temporadaId,
        CancellationToken cancellationToken = default)
    {
        return _repository.ContarNominaActivaAsync(temporadaId, cancellationToken);
    }

    public async Task<OperationResult<NominaEquipoDto>> ObtenerNominaAsync(
        long equipoId,
        CancellationToken cancellationToken = default)
    {
        var equipo = await _repository.GetEquipoAsync(equipoId, cancellationToken);
        if (equipo is null)
            return OperationResult<NominaEquipoDto>.Fail("El equipo no existe.");

        var membresias = await _repository.ListMembresiasAsync(equipoId, cancellationToken);
        return OperationResult<NominaEquipoDto>.Ok(MapNomina(equipo, membresias));
    }

    public Task<OperationResult<JugadorNominaDto>> AgregarJugadorAsync(
        long equipoId,
        string documento,
        string nombres,
        DateOnly? fechaNacimiento,
        bool consideradoJuvenil,
        bool habilitado,
        CancellationToken cancellationToken = default)
    {
        var documentoLimpio = ReglasJugador.NormalizarDocumento(documento);
        if (documentoLimpio is null)
        {
            return Task.FromResult(OperationResult<JugadorNominaDto>.Fail(
                $"El documento debe tener entre {ReglasJugador.MinimoDocumento} y {ReglasJugador.MaximoDocumento} letras o números."));
        }

        var nombresLimpios = ReglasJugador.NormalizarNombres(nombres);
        if (nombresLimpios is null)
        {
            return Task.FromResult(OperationResult<JugadorNominaDto>.Fail(
                "El nombre del jugador es obligatorio y no puede superar los 180 caracteres."));
        }

        var hoy = ReglasJugador.Hoy();
        if (fechaNacimiento is DateOnly nacimiento && (nacimiento > hoy || nacimiento.Year < 1920))
        {
            return Task.FromResult(OperationResult<JugadorNominaDto>.Fail(
                "La fecha de nacimiento no es válida."));
        }

        if (consideradoJuvenil && ReglasJugador.EsMayorDe50(fechaNacimiento, hoy))
        {
            return Task.FromResult(OperationResult<JugadorNominaDto>.Fail(
                "Un jugador mayor de 50 años no se registra como juvenil."));
        }

        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var equipo = await _repository.GetEquipoAsync(equipoId, ct);
            if (equipo is null)
                return OperationResult<JugadorNominaDto>.Fail("El equipo no existe.");

            var ahora = DateTime.UtcNow;
            var jugador = await _repository.GetJugadorPorDocumentoAsync(documentoLimpio, ct);
            var nacimiento = fechaNacimiento ?? jugador?.FechaNacimiento;
            if (consideradoJuvenil && ReglasJugador.EsMayorDe50(nacimiento, hoy))
            {
                return OperationResult<JugadorNominaDto>.Fail(
                    "Un jugador mayor de 50 años no se registra como juvenil.");
            }

            if (jugador is not null)
            {
                var abierta = await _repository.GetMembresiaAbiertaEnTemporadaAsync(jugador.Id, equipo.TemporadaId, ct);
                if (abierta is not null)
                {
                    var mensaje = abierta.EquipoId == equipo.Id
                        ? $"{jugador.Nombres} ya está en la nómina de este equipo."
                        : $"{jugador.Nombres} ya está en la nómina de {abierta.Equipo.Nombre} en esta temporada.";
                    return OperationResult<JugadorNominaDto>.Fail(mensaje);
                }

                if (await _repository.ExisteAltaEnFechaAsync(jugador.Id, equipo.Id, hoy, ct))
                {
                    return OperationResult<JugadorNominaDto>.Fail(
                        $"{jugador.Nombres} ya tuvo un alta hoy en este equipo.");
                }

                jugador.Nombres = nombresLimpios;
                jugador.Documento = documentoLimpio;
                if (fechaNacimiento is not null)
                    jugador.FechaNacimiento = fechaNacimiento;
                jugador.Activo = true;
                jugador.ActualizadoEn = ahora;
                jugador.ActualizadoPor = _currentUser.UserId;
            }
            else
            {
                jugador = new Jugadore
                {
                    Documento = documentoLimpio,
                    Nombres = nombresLimpios,
                    FechaNacimiento = fechaNacimiento,
                    Activo = true,
                    CreadoEn = ahora,
                    ActualizadoEn = ahora,
                    CreadoPor = _currentUser.UserId,
                    ActualizadoPor = _currentUser.UserId
                };
                await _repository.AddJugadorAsync(jugador, ct);
            }

            var membresia = new JugadoresEquipo
            {
                Jugador = jugador,
                EquipoId = equipo.Id,
                FechaAlta = hoy,
                Habilitado = habilitado,
                ConsideradoJuvenil = consideradoJuvenil,
                CreadoEn = ahora,
                ActualizadoEn = ahora,
                CreadoPor = _currentUser.UserId,
                ActualizadoPor = _currentUser.UserId
            };
            await _repository.AddMembresiaAsync(membresia, ct);
            await _repository.SaveChangesAsync(ct);

            return OperationResult<JugadorNominaDto>.Ok(
                MapJugador(membresia, hoy),
                $"{jugador.Nombres} quedó en la nómina de {equipo.Nombre}.");
        }, cancellationToken);
    }

    public Task<OperationResult> CambiarHabilitacionAsync(
        long membresiaId,
        bool habilitado,
        CancellationToken cancellationToken = default)
    {
        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var membresia = await _repository.GetMembresiaAsync(membresiaId, ct);
            if (membresia is null)
                return OperationResult.Fail("El jugador no está en esta nómina.");

            if (membresia.FechaBaja is not null)
                return OperationResult.Fail("Este jugador ya no está en la nómina.");

            membresia.Habilitado = habilitado;
            Touched(membresia);
            await _repository.SaveChangesAsync(ct);
            var estado = habilitado ? "puede jugar" : "quedó sin habilitación";
            var nombre = membresia.Jugador.Nombres;
            return OperationResult.Ok(habilitado ? $"{nombre} {estado}." : $"{nombre} {estado}.");
        }, cancellationToken);
    }

    public Task<OperationResult> CambiarCondicionJuvenilAsync(
        long membresiaId,
        bool consideradoJuvenil,
        CancellationToken cancellationToken = default)
    {
        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var membresia = await _repository.GetMembresiaAsync(membresiaId, ct);
            if (membresia is null)
                return OperationResult.Fail("El jugador no está en esta nómina.");

            if (membresia.FechaBaja is not null)
                return OperationResult.Fail("Este jugador ya no está en la nómina.");

            if (consideradoJuvenil && ReglasJugador.EsMayorDe50(membresia.Jugador.FechaNacimiento, ReglasJugador.Hoy()))
            {
                return OperationResult.Fail("Un jugador mayor de 50 años no se considera juvenil.");
            }

            membresia.ConsideradoJuvenil = consideradoJuvenil;
            Touched(membresia);
            await _repository.SaveChangesAsync(ct);
            var condicion = consideradoJuvenil ? "juvenil" : "sin condición de juvenil";
            return OperationResult.Ok($"{membresia.Jugador.Nombres} quedó {condicion}.");
        }, cancellationToken);
    }

    public Task<OperationResult> DarDeBajaAsync(long membresiaId, CancellationToken cancellationToken = default)
    {
        return _transactionRunner.ExecuteAsync(async ct =>
        {
            var membresia = await _repository.GetMembresiaAsync(membresiaId, ct);
            if (membresia is null)
                return OperationResult.Fail("El jugador no está en esta nómina.");

            if (membresia.FechaBaja is not null)
                return OperationResult.Fail("Este jugador ya salió de la nómina.");

            membresia.FechaBaja = ReglasJugador.Hoy();
            Touched(membresia);
            await _repository.SaveChangesAsync(ct);
            return OperationResult.Ok($"{membresia.Jugador.Nombres} salió de la nómina.");
        }, cancellationToken);
    }

    private void Touched(JugadoresEquipo membresia)
    {
        membresia.ActualizadoEn = DateTime.UtcNow;
        membresia.ActualizadoPor = _currentUser.UserId;
    }

    private static NominaEquipoDto MapNomina(Equipo equipo, IReadOnlyList<JugadoresEquipo> membresias)
    {
        var hoy = ReglasJugador.Hoy();
        return new NominaEquipoDto
        {
            EquipoId = equipo.Id,
            NombreEquipo = equipo.Nombre,
            NombreCategoria = equipo.Categoria.Nombre,
            NombreTemporada = equipo.Temporada.Nombre,
            TemporadaActiva = equipo.Temporada.Activa,
            Jugadores = membresias.Select(m => MapJugador(m, hoy)).ToList()
        };
    }

    private static JugadorNominaDto MapJugador(JugadoresEquipo membresia, DateOnly hoy)
    {
        var nacimiento = membresia.Jugador.FechaNacimiento;
        return new JugadorNominaDto
        {
            MembresiaId = membresia.Id,
            JugadorId = membresia.JugadorId,
            Documento = membresia.Jugador.Documento,
            Nombres = membresia.Jugador.Nombres,
            FechaNacimiento = nacimiento,
            Edad = ReglasJugador.Edad(nacimiento, hoy),
            MayorDe50 = ReglasJugador.EsMayorDe50(nacimiento, hoy),
            ConsideradoJuvenil = membresia.ConsideradoJuvenil,
            Habilitado = membresia.Habilitado,
            FechaAlta = membresia.FechaAlta,
            FechaBaja = membresia.FechaBaja
        };
    }
}
