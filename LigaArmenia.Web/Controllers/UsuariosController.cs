using LigaArmenia.Application.DTOs.Users;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Web.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaArmenia.Web.Controllers;

[Authorize]
public sealed class UsuariosController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly IRolRepository _rolRepository;

    public UsuariosController(
        IUsuarioService usuarioService,
        IRolRepository rolRepository)
    {
        _usuarioService = usuarioService;
        _rolRepository = rolRepository;
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.UsuariosConsultar)]
    public async Task<IActionResult> Index(string? busqueda, long? rolId, bool? activo, CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioService.SearchAsync(
            new UsuarioFilterDto
            {
                Busqueda = busqueda,
                RolId = rolId,
                Activo = activo
            },
            cancellationToken);

        var roles = await LoadRoleOptionsAsync(cancellationToken);

        var vm = new UsuarioIndexViewModel
        {
            Busqueda = busqueda,
            RolId = rolId,
            Activo = activo,
            Usuarios = usuarios,
            RolesDisponibles = roles
        };

        return View(vm);
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.UsuariosCrear)]
    public async Task<IActionResult> Create(bool preselectVocal = false, CancellationToken cancellationToken = default)
    {
        var roles = await LoadRoleOptionsAsync(cancellationToken);
        var preselectedIds = new List<long>();

        if (preselectVocal)
        {
            var vocalRole = roles.FirstOrDefault(r =>
                string.Equals(r.Nombre, SystemRoles.Vocal, StringComparison.OrdinalIgnoreCase));
            if (vocalRole is not null)
                preselectedIds.Add(vocalRole.Id);
        }

        var vm = new UsuarioCreateViewModel
        {
            Activo = true,
            RoleIds = preselectedIds,
            RolesDisponibles = roles
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.UsuariosCrear)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.RolesDisponibles = await LoadRoleOptionsAsync(cancellationToken);
            return View(model);
        }

        var result = await _usuarioService.CreateAsync(
            new CreateUsuarioDto
            {
                Nombres = model.Nombres,
                Correo = model.Correo,
                Password = model.Password,
                Activo = model.Activo,
                RoleIds = model.RoleIds,
                TelefonoVocal = model.TelefonoVocal
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            model.RolesDisponibles = await LoadRoleOptionsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message ?? "Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.UsuariosEditar)]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.GetByIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            TempData["ErrorMessage"] = "El usuario solicitado no existe.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new UsuarioEditViewModel
        {
            Id = usuario.Id,
            Nombres = usuario.Nombres,
            Correo = usuario.Correo,
            Activo = usuario.Activo,
            TienePerfilVocal = usuario.TienePerfilVocal,
            VocalActivo = usuario.VocalActivo,
            TelefonoVocal = usuario.TelefonoVocal,
            RoleIds = usuario.RoleIds.ToList(),
            RolesDisponibles = await LoadRoleOptionsAsync(cancellationToken)
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.UsuariosEditar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsuarioEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.RolesDisponibles = await LoadRoleOptionsAsync(cancellationToken);
            return View(model);
        }

        var result = await _usuarioService.UpdateAsync(
            new UpdateUsuarioDto
            {
                Id = model.Id,
                Nombres = model.Nombres,
                Correo = model.Correo,
                RoleIds = model.RoleIds,
                TelefonoVocal = model.TelefonoVocal
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            model.RolesDisponibles = await LoadRoleOptionsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message ?? "Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.UsuariosDesactivar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(long id, bool activo, CancellationToken cancellationToken)
    {
        var result = await _usuarioService.SetActiveStatusAsync(id, activo, cancellationToken);
        if (!result.Succeeded)
            TempData["ErrorMessage"] = result.Message ?? "No se pudo actualizar el estado del usuario.";
        else
            TempData["SuccessMessage"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = SystemPermissions.UsuariosEditar)]
    public async Task<IActionResult> ResetPassword(long id, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.GetByIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            TempData["ErrorMessage"] = "El usuario solicitado no existe.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new UsuarioResetPasswordViewModel
        {
            UsuarioId = usuario.Id,
            Nombres = usuario.Nombres,
            Correo = usuario.Correo
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = SystemPermissions.UsuariosEditar)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(UsuarioResetPasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _usuarioService.ResetPasswordAsync(
            new ResetPasswordDto
            {
                UsuarioId = model.UsuarioId,
                NewPassword = model.NewPassword
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            model.NewPassword = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message ?? "Contraseña restablecida correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<RoleOptionViewModel>> LoadRoleOptionsAsync(CancellationToken cancellationToken)
    {
        var roles = await _rolRepository.GetAllAsync(cancellationToken);
        return roles.Select(r => new RoleOptionViewModel
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion
        }).ToList();
    }
}
