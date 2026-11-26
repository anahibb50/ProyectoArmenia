using System.Security.Claims;
using LigaArmenia.Application.Interfaces.Security;
using Microsoft.AspNetCore.Authorization;

namespace LigaArmenia.Web.Security;

public sealed class VocaliaAssignmentAuthorizationHandler
    : AuthorizationHandler<VocaliaAssignmentRequirement, VocaliaResourceTarget>
{
    private readonly IVocaliaOwnershipService _ownershipService;

    public VocaliaAssignmentAuthorizationHandler(IVocaliaOwnershipService ownershipService)
    {
        _ownershipService = ownershipService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        VocaliaAssignmentRequirement requirement,
        VocaliaResourceTarget resource)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var rawUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(rawUserId, out var userId))
            return;

        var allowed = resource.ResourceType switch
        {
            VocaliaResourceType.Partido => await _ownershipService.IsUserAssignedToMatchAsync(userId, resource.ResourceId),
            VocaliaResourceType.Vocalia => await _ownershipService.CanUserAccessVocaliaAsync(userId, resource.ResourceId),
            _ => false
        };

        if (allowed)
            context.Succeed(requirement);
    }
}
