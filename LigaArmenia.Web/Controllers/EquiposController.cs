using LigaArmenia.Application.DTOs.Campeonato;
using LigaArmenia.Application.DTOs.Nomina;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Web.ViewModels.Equipos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaArmenia.Web.Controllers;

[Authorize(Policy = SystemPermissions.EquiposConsultar)]
public sealed class EquiposController : Controller
{
    private readonly ICampeonatoService _campeonatoService;
    private readonly INominaService _nominaService;

    public EquiposController(ICampeonatoService campeonatoService, INominaService nominaService)
    {
        _campeonatoService = campeonatoService;
        _nominaService = nominaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? temporada, string? categoria, CancellationToken cancellationToken)
    {
        var panel = await _campeonatoService.ObtenerPanelAsync(temporada, cancellationToken);
        if (!panel.Succeeded || panel.Data is null)
        {
            TempData["ErrorMessage"] = panel.Message ?? "No se pudo cargar la temporada.";
            return View(new EquiposIndexViewModel());
        }

        var conteos = await _nominaService.ContarNominaActivaAsync(panel.Data.Temporada.TemporadaId, cancellationToken);
        return View(MapIndex(panel.Data, conteos, categoria));
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.EquiposGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistrarEquipoEnTemporadaViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Indica la temporada, la categoría y el nombre del equipo.";
            return RedirectToAction(nameof(Index), new { temporada = model.Temporada, categoria = model.CategoriaFiltro });
        }

        var result = await _campeonatoService.RegistrarEquipoAsync(
            model.Temporada,
            model.CodigoCategoria,
            model.Nombre,
            cancellationToken);

        if (!result.Succeeded)
            TempData["ErrorMessage"] = result.Message;
        else
            TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Index), new { temporada = model.Temporada, categoria = model.CategoriaFiltro });
    }

    [HttpGet]
    public async Task<IActionResult> Nomina(long id, CancellationToken cancellationToken)
    {
        var nomina = await _nominaService.ObtenerNominaAsync(id, cancellationToken);
        if (!nomina.Succeeded || nomina.Data is null)
        {
            TempData["ErrorMessage"] = nomina.Message ?? "No se pudo abrir la nómina.";
            return RedirectToAction(nameof(Index));
        }

        return View(MapNomina(nomina.Data));
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.JugadoresGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarJugador(AgregarJugadorViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Revisa el documento, el nombre y la fecha de nacimiento.";
            return RedirectToAction(nameof(Nomina), new { id = model.EquipoId });
        }

        var result = await _nominaService.AgregarJugadorAsync(
            model.EquipoId,
            model.Documento,
            model.Nombres,
            model.FechaNacimiento,
            model.ConsideradoJuvenil,
            model.Habilitado,
            cancellationToken);

        if (!result.Succeeded)
            TempData["ErrorMessage"] = result.Message;
        else
            TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Nomina), new { id = model.EquipoId });
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.JugadoresGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarHabilitacion(long id, long equipoId, bool habilitado, CancellationToken cancellationToken)
    {
        var result = await _nominaService.CambiarHabilitacionAsync(id, habilitado, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Nomina), new { id = equipoId });
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.JugadoresGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarJuvenil(long id, long equipoId, bool juvenil, CancellationToken cancellationToken)
    {
        var result = await _nominaService.CambiarCondicionJuvenilAsync(id, juvenil, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Nomina), new { id = equipoId });
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.JugadoresGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DarDeBaja(long id, long equipoId, CancellationToken cancellationToken)
    {
        var result = await _nominaService.DarDeBajaAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction(nameof(Nomina), new { id = equipoId });
    }

    private static EquiposIndexViewModel MapIndex(
        PanelCampeonatoDto panel,
        IReadOnlyDictionary<long, int> conteos,
        string? categoria)
    {
        var equipos = panel.Equipos;
        var filas = panel.Temporada.Categorias
            .SelectMany(division => equipos
                .Where(e => e.CategoriaId == division.Id)
                .Select(e => new EquipoFilaViewModel
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    CodigoCategoria = division.Codigo,
                    Categoria = division.Nombre,
                    Jugadores = conteos.TryGetValue(e.Id, out var total) ? total : 0,
                    Activo = e.Activo
                }))
            .OrderBy(e => e.Categoria)
            .ThenBy(e => e.Nombre)
            .ToList();

        var codigo = (categoria ?? string.Empty).Trim();
        var seleccionada = panel.Temporada.Categorias
            .FirstOrDefault(c => string.Equals(c.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
            ?.Codigo ?? string.Empty;
        var visibles = string.IsNullOrEmpty(seleccionada)
            ? filas
            : filas.Where(e => string.Equals(e.CodigoCategoria, seleccionada, StringComparison.OrdinalIgnoreCase)).ToList();

        return new EquiposIndexViewModel
        {
            NombreTemporada = panel.Temporada.Nombre,
            EsTemporadaActual = panel.Temporada.Activa,
            CategoriaSeleccionada = seleccionada,
            TotalEquipos = filas.Count,
            Temporadas = panel.Temporadas.Select(t => new TemporadaOpcionViewModel
            {
                Nombre = t.Nombre,
                Activa = t.Activa
            }).ToList(),
            Equipos = visibles,
            Categorias = panel.Temporada.Categorias.Select(c =>
            {
                var registrados = equipos.Count(e => e.CategoriaId == c.Id);
                return new CategoriaOpcionViewModel
                {
                    Codigo = c.Codigo,
                    Nombre = c.Nombre,
                    CupoLleno = c.CupoEquipos is short cupo && registrados >= cupo
                };
            }).ToList()
        };
    }

    private static NominaViewModel MapNomina(NominaEquipoDto nomina)
    {
        JugadorFilaViewModel Map(JugadorNominaDto jugador) => new()
        {
            MembresiaId = jugador.MembresiaId,
            Documento = jugador.Documento,
            Nombres = jugador.Nombres,
            FechaNacimiento = jugador.FechaNacimiento,
            Edad = jugador.Edad,
            MayorDe50 = jugador.MayorDe50,
            ConsideradoJuvenil = jugador.ConsideradoJuvenil,
            Habilitado = jugador.Habilitado,
            FechaAlta = jugador.FechaAlta,
            FechaBaja = jugador.FechaBaja
        };

        return new NominaViewModel
        {
            EquipoId = nomina.EquipoId,
            NombreEquipo = nomina.NombreEquipo,
            NombreCategoria = nomina.NombreCategoria,
            NombreTemporada = nomina.NombreTemporada,
            Activos = nomina.Jugadores.Where(j => j.FechaBaja is null).Select(Map).ToList(),
            Bajas = nomina.Jugadores.Where(j => j.FechaBaja is not null).Select(Map).ToList()
        };
    }
}
