using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace LigaArmenia.Web.Security;

/// <summary>
/// Resuelve dinámicamente políticas con formato "modulo.accion" para cualquier
/// controlador o acción decorada con [Authorize(Policy = "modulo.accion")].
/// </summary>
public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var existingPolicy = await _fallbackPolicyProvider.GetPolicyAsync(policyName);
        if (existingPolicy is not null)
            return existingPolicy;

        if (!string.IsNullOrWhiteSpace(policyName) && policyName.Contains('.'))
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            return policy;
        }

        return null;
    }
}
