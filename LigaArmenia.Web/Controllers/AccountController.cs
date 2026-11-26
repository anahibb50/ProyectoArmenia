using System.Security.Claims;
using LigaArmenia.Application.DTOs.Auth;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Web.Extensions;
using LigaArmenia.Web.Services;
using LigaArmenia.Web.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LigaArmenia.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IPostLoginRedirectResolver _redirectResolver;

    public AccountController(
        IAuthService authService,
        IPostLoginRedirectResolver redirectResolver)
    {
        _authService = authService;
        _redirectResolver = redirectResolver;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting(SecurityExtensions.LoginRateLimitPolicy)]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.AuthenticateAsync(
            new LoginRequestDto
            {
                Correo = model.Correo,
                Password = model.Password,
                Recordarme = model.Recordarme
            },
            AuthChannel.WebPortal,
            clientIp,
            cancellationToken);

        if (!result.Succeeded || result.User is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Correo o contraseña incorrectos.");
            model.Password = string.Empty;
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
            new(ClaimTypes.Name, result.User.Nombres),
            new(ClaimTypes.Email, result.User.Correo)
        };

        foreach (var role in result.User.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.Recordarme,
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = model.Recordarme
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(8)
            });

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return LocalRedirect(model.ReturnUrl);

        var (controller, action) = _redirectResolver.Resolve(result.User);
        return RedirectToAction(action, controller);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "Has cerrado sesión correctamente.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }
}
