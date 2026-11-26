using System.Security.Claims;
using LigaArmenia.Application.Interfaces.Security;
using Microsoft.AspNetCore.Authorization;

namespace LigaArmenia.Web.Security;

/// <summary>
/// Evalúa políticas basadas en permisos ([Authorize(Policy = "modulo.accion")])
/// directamente en el servidor contra el estado de seguridad vigente del usuario.
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserSecurityStateService _securityStateService;

    public PermissionAuthorizationHandler(IUserSecurityStateService securityStateService)
    {
        _securityStateService = securityStateService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var rawUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(rawUserId, out var userId))
            return;

        var state = await _securityStateService.GetStateAsync(userId);
        if (state is null || !state.Activo)
            return;

        if (state.HasPermission(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
