using System.Security.Claims;
using LigaArmenia.Application.DTOs.Auth;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Web.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LigaArmenia.Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
public sealed class AuthApiController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserSecurityStateService _securityStateService;

    public AuthApiController(
        IAuthService authService,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserSecurityStateService securityStateService)
    {
        _authService = authService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _securityStateService = securityStateService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(SecurityExtensions.LoginRateLimitPolicy)]
    [ProducesResponseType(typeof(ApiLoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] ApiLoginRequestDto request, CancellationToken cancellationToken)
    {
        if (!_jwtTokenGenerator.IsConfigured)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new { message = "La autenticación JWT no está configurada en el servidor." });
        }

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var authResult = await _authService.AuthenticateAsync(
            new LoginRequestDto
            {
                Correo = request.Correo,
                Password = request.Password
            },
            AuthChannel.MobileApi,
            clientIp,
            cancellationToken);

        if (!authResult.Succeeded || authResult.User is null)
        {
            if (authResult.IsLockedOut)
                return StatusCode(StatusCodes.Status429TooManyRequests, new { message = authResult.ErrorMessage });

            if (authResult.IsChannelForbidden)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = authResult.ErrorMessage });

            return Unauthorized(new { message = authResult.ErrorMessage ?? "Correo o contraseña incorrectos." });
        }

        var token = _jwtTokenGenerator.GenerateToken(authResult.User);

        var response = new ApiLoginResponseDto
        {
            AccessToken = token.AccessToken,
            TokenType = token.TokenType,
            ExpiresAtUtc = token.ExpiresAtUtc,
            Usuario = new UserProfileApiDto
            {
                Id = authResult.User.Id,
                Nombres = authResult.User.Nombres,
                Correo = authResult.User.Correo,
                Activo = true,
                VocalId = authResult.User.VocalId,
                Roles = authResult.User.Roles,
                Permisos = authResult.User.Permisos
            }
        };

        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(UserProfileApiDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(rawUserId, out var userId))
            return Unauthorized(new { message = "Token inválido." });

        var state = await _securityStateService.GetStateAsync(userId, cancellationToken);
        if (state is null || !state.Activo)
            return Unauthorized(new { message = "Cuenta inactiva o inexistente." });

        if (!state.CanAccessMobileApp)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Sin autorización para la aplicación móvil." });

        return Ok(new UserProfileApiDto
        {
            Id = state.UsuarioId,
            Nombres = state.Nombres,
            Correo = state.Correo,
            Activo = state.Activo,
            VocalId = state.VocalId,
            Roles = state.Roles.OrderBy(r => r).ToList(),
            Permisos = state.Permissions.OrderBy(p => p).ToList()
        });
    }
}
