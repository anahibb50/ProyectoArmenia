using LigaArmenia.Application.DTOs.Vocales;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Web.ViewModels.Vocales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaArmenia.Web.Controllers;

[Authorize]
public sealed class VocalesController : Controller
{
    private readonly IVocalService _vocalService;

    public VocalesController(IVocalService vocalService)
    {
        _vocalService = vocalService;
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.VocalesConsultar)]
    public async Task<IActionResult> Index(
        string? busqueda,
        bool? activo,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _vocalService.SearchPagedAsync(
            new VocalFilterDto
            {
                Busqueda = busqueda,
                Activo = activo,
                Page = page,
                PageSize = pageSize
            },
            cancellationToken);

        var vm = new VocalIndexViewModel
        {
            Busqueda = busqueda,
            Activo = activo,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalFiltered = result.TotalFiltered,
            TotalPages = result.TotalPages,
            TotalRegistrados = result.TotalRegistrados,
            TotalActivos = result.TotalActivos,
            TotalInactivos = result.TotalInactivos,
            Vocales = result.Items
        };

        return View(vm);
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.VocalesEditar)]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken)
    {
        var vocal = await _vocalService.GetByIdAsync(id, cancellationToken);
        if (vocal is null)
        {
            TempData["ErrorMessage"] = "El perfil de vocal solicitado no existe.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new VocalEditViewModel
        {
            Id = vocal.Id,
            UsuarioId = vocal.UsuarioId,
            Correo = vocal.Correo,
            Activo = vocal.Activo,
            UsuarioActivo = vocal.UsuarioActivo,
            TieneRolVocal = vocal.TieneRolVocal,
            CreadoEn = vocal.CreadoEn,
            Nombres = vocal.Nombres,
            Telefono = vocal.Telefono
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.VocalesEditar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VocalEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _vocalService.UpdateAsync(
            new UpdateVocalDto
            {
                Id = model.Id,
                Nombres = model.Nombres,
                Telefono = model.Telefono
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        TempData["SuccessMessage"] = result.Message ?? "Vocal actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.VocalesActivar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(
        long id,
        string? busqueda,
        bool? activo,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _vocalService.SetActiveStatusAsync(id, true, cancellationToken);
        if (!result.Succeeded)
            TempData["ErrorMessage"] = result.Message ?? "No se pudo activar el perfil del vocal.";
        else
            TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Index), new { busqueda, activo, page });
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.VocalesDesactivar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(
        long id,
        string? busqueda,
        bool? activo,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await _vocalService.SetActiveStatusAsync(id, false, cancellationToken);
        if (!result.Succeeded)
            TempData["WarningMessage"] = result.Message ?? "No se pudo desactivar el perfil del vocal.";
        else
            TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Index), new { busqueda, activo, page });
    }
}
