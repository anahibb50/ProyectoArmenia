using LigaArmenia.Application.DTOs.Roles;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Web.ViewModels.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaArmenia.Web.Controllers;

[Authorize(Roles = SystemRoles.Administrador)]
public sealed class RolesController : Controller
{
    private readonly IRolPermisoService _rolPermisoService;

    public RolesController(IRolPermisoService rolPermisoService)
    {
        _rolPermisoService = rolPermisoService;
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.RolesConsultar)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var roles = await _rolPermisoService.GetAllRolesAsync(cancellationToken);
        return View(new RolIndexViewModel { Roles = roles });
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.PermisosConsultar)]
    public async Task<IActionResult> ManagePermissions(long id, CancellationToken cancellationToken)
    {
        var detail = await _rolPermisoService.GetRoleDetailAsync(id, cancellationToken);
        if (detail is null)
        {
            TempData["ErrorMessage"] = "El rol solicitado no existe.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new RolManagePermissionsViewModel
        {
            RolId = detail.Id,
            Nombre = detail.Nombre,
            Descripcion = detail.Descripcion,
            EsAdministrador = detail.EsAdministrador,
            SelectedPermissionIds = detail.Modulos
                .SelectMany(m => m.Permisos)
                .Where(p => p.Asignado)
                .Select(p => p.Id)
                .ToList(),
            Modulos = detail.Modulos,
            Usuarios = detail.Usuarios
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.PermisosAsignar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManagePermissions(RolManagePermissionsViewModel model, CancellationToken cancellationToken)
    {
        var result = await _rolPermisoService.UpdateRolePermissionsAsync(
            new UpdateRolPermisosDto
            {
                RolId = model.RolId,
                PermisoIds = model.SelectedPermissionIds ?? new List<long>()
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message ?? "No se pudieron actualizar los permisos del rol.";
            return RedirectToAction(nameof(ManagePermissions), new { id = model.RolId });
        }

        TempData["SuccessMessage"] = result.Message ?? "Permisos actualizados correctamente.";
        return RedirectToAction(nameof(ManagePermissions), new { id = model.RolId });
    }
}
