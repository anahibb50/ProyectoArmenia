using LigaArmenia.Application.DTOs.Campeonato;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Web.ViewModels.Campeonato;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaArmenia.Web.Controllers;

[Authorize]
public sealed class CampeonatosController : Controller
{
    private readonly ICampeonatoService _campeonatoService;

    public CampeonatosController(ICampeonatoService campeonatoService)
    {
        _campeonatoService = campeonatoService;
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.CampeonatosConsultar)]
    public async Task<IActionResult> Index(string? temporada, CancellationToken cancellationToken)
    {
        var panel = await _campeonatoService.ObtenerPanelAsync(temporada, cancellationToken);
        if (!panel.Succeeded || panel.Data is null)
        {
            TempData["ErrorMessage"] = panel.Message ?? "No se pudo cargar la temporada.";
            return View(new CampeonatoIndexViewModel());
        }

        return View(MapIndex(panel.Data));
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.EquiposGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarEquipo(RegistrarEquipoViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Indica la temporada, la categoría y el nombre del equipo.";
            return RedirectToAction(nameof(Index), new { temporada = model.Temporada });
        }

        var result = await _campeonatoService.RegistrarEquipoAsync(
            model.Temporada,
            model.CodigoCategoria,
            model.Nombre,
            cancellationToken);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index), new { temporada = model.Temporada });
        }

        TempData["SuccessMessage"] = result.Message ?? "Equipo registrado.";
        return RedirectToAction(nameof(Index), new { temporada = model.Temporada });
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.CampeonatosGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AbrirSiguiente(CancellationToken cancellationToken)
    {
        var result = await _campeonatoService.AbrirSiguienteTemporadaAsync(cancellationToken);
        if (!result.Succeeded || string.IsNullOrWhiteSpace(result.Data))
        {
            TempData["ErrorMessage"] = result.Message ?? "No se pudo abrir la temporada.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index), new { temporada = result.Data });
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.CampeonatosGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarActual(string temporada, CancellationToken cancellationToken)
    {
        var result = await _campeonatoService.MarcarTemporadaActualAsync(temporada, cancellationToken);
        if (!result.Succeeded)
            TempData["ErrorMessage"] = result.Message;
        else
            TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Index), new { temporada });
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.CampeonatosGestionar)]
    public async Task<IActionResult> Configurar(string codigo, string temporada, CancellationToken cancellationToken)
    {
        var categoria = await BuscarConfigurableAsync(codigo, temporada, cancellationToken);
        if (categoria is null)
            return RedirectToAction(nameof(Index), new { temporada });

        return View(MapConfigurar(categoria, temporada));
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.CampeonatosGestionar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Configurar(ConfigurarDivisionViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var etapas = (model.Etapas ?? [])
            .Where(e => !string.IsNullOrWhiteSpace(e.Nombre))
            .OrderBy(e => e.Numero)
            .ToList();

        if (etapas.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Indica al menos la etapa 1.");
            return View(model);
        }

        if (etapas.Any(e => e.Numero > 1) && etapas.All(e => e.Numero != 1))
        {
            ModelState.AddModelError(string.Empty, "La etapa 1 es la primera que debe existir.");
            return View(model);
        }

        if (etapas.Any(e => e.Numero == 3) && etapas.All(e => e.Numero != 2))
        {
            ModelState.AddModelError(string.Empty, "Para usar la etapa 3, completa también la etapa 2.");
            return View(model);
        }

        var result = await _campeonatoService.ConfigurarDivisionAsync(
            model.Temporada,
            model.Codigo,
            model.CupoEquipos!.Value,
            etapas.Select(e => new EtapaSolicitudDto
            {
                Numero = e.Numero,
                Nombre = e.Nombre ?? string.Empty,
                Formato = e.Formato
            }).ToList(),
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        TempData["SuccessMessage"] = result.Message ?? "División configurada.";
        return RedirectToAction(nameof(Index), new { temporada = model.Temporada });
    }

    private async Task<CategoriaDto?> BuscarConfigurableAsync(string codigo, string temporada, CancellationToken cancellationToken)
    {
        var panel = await _campeonatoService.ObtenerPanelAsync(temporada, cancellationToken);
        if (!panel.Succeeded || panel.Data is null)
        {
            TempData["ErrorMessage"] = panel.Message ?? "No se pudo cargar la temporada.";
            return null;
        }

        var categoria = panel.Data.Temporada.Categorias
            .FirstOrDefault(c => string.Equals(c.Codigo, codigo, StringComparison.OrdinalIgnoreCase));

        if (categoria is null || !categoria.EsConfigurable)
        {
            TempData["ErrorMessage"] = "Sénior A y Sénior B ya tienen cupo y etapas fijos.";
            return null;
        }

        return categoria;
    }

    private static CampeonatoIndexViewModel MapIndex(PanelCampeonatoDto panel)
    {
        var equipos = panel.Equipos;
        return new CampeonatoIndexViewModel
        {
            TemporadaId = panel.Temporada.TemporadaId,
            NombreTemporada = panel.Temporada.Nombre,
            EsTemporadaActual = panel.Temporada.Activa,
            SiguienteTemporada = panel.SiguienteTemporada,
            Temporadas = panel.Temporadas.Select(t => new TemporadaOpcionViewModel
            {
                Nombre = t.Nombre,
                Activa = t.Activa
            }).ToList(),
            Categorias = panel.Temporada.Categorias.Select(c =>
            {
                var deLaCategoria = equipos.Where(e => e.CategoriaId == c.Id).ToList();
                return new CategoriaPanelViewModel
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    Nombre = c.Nombre,
                    Disciplina = c.Disciplina,
                    Serie = c.Serie?.ToString(),
                    CupoEquipos = c.CupoEquipos,
                    EquiposRegistrados = deLaCategoria.Count,
                    PuntosVictoria = c.PuntosVictoria,
                    PuntosEmpate = c.PuntosEmpate,
                    PuntosDerrota = c.PuntosDerrota,
                    EsConfigurable = c.EsConfigurable,
                    CupoLleno = c.CupoEquipos is short cupo && deLaCategoria.Count >= cupo,
                    Etapas = c.Etapas.Select(e => new EtapaPanelViewModel
                    {
                        Numero = e.Numero,
                        Nombre = e.Nombre,
                        Formato = e.Formato
                    }).ToList(),
                    Equipos = deLaCategoria.Select(e => new EquipoPanelViewModel
                    {
                        Id = e.Id,
                        Nombre = e.Nombre,
                        Activo = e.Activo
                    }).ToList()
                };
            }).ToList()
        };
    }

    private static ConfigurarDivisionViewModel MapConfigurar(CategoriaDto categoria, string temporada)
    {
        var etapas = new List<EtapaFormViewModel>
        {
            new() { Numero = 1, Formato = "TODOS_CONTRA_TODOS" },
            new() { Numero = 2, Formato = "TODOS_CONTRA_TODOS" },
            new() { Numero = 3, Formato = "TODOS_CONTRA_TODOS" }
        };

        foreach (var etapa in categoria.Etapas)
        {
            var slot = etapas.FirstOrDefault(e => e.Numero == etapa.Numero);
            if (slot is null)
                continue;

            slot.Nombre = etapa.Nombre;
            slot.Formato = etapa.Formato;
        }

        return new ConfigurarDivisionViewModel
        {
            Temporada = temporada,
            Codigo = categoria.Codigo,
            NombreCategoria = categoria.Nombre,
            CupoEquipos = categoria.CupoEquipos,
            Etapas = etapas
        };
    }
}
