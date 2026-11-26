namespace LigaArmenia.Application.Interfaces.Security;

public sealed record UserSecurityState(
    long UsuarioId,
    string Nombres,
    string Correo,
    bool Activo,
    IReadOnlySet<string> Roles,
    IReadOnlySet<string> Permissions,
    long? VocalId)
{
    public bool IsAdmin => Roles.Contains(Domain.Constants.SystemRoles.Administrador);

    public bool CanAccessWebPortal =>
        Activo && Roles.Any(r => Domain.Constants.SystemRoles.WebPortalAllowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase));

    public bool CanAccessMobileApp =>
        Activo && Roles.Any(r => Domain.Constants.SystemRoles.MobileAllowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase));

    public bool HasPermission(string permissionCode) =>
        Activo && (IsAdmin || Permissions.Contains(permissionCode));
}

public interface IUserSecurityStateService
{
    Task<UserSecurityState?> GetStateAsync(long usuarioId, CancellationToken cancellationToken = default);
    void InvalidateUser(long usuarioId);
    void InvalidateUsers(IEnumerable<long> usuarioIds);
}
