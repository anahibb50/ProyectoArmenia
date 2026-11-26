using LigaArmenia.Application.DTOs.Auth;
using LigaArmenia.Domain.Constants;

namespace LigaArmenia.Web.Services;

public interface IPostLoginRedirectResolver
{
    (string Controller, string Action) Resolve(AuthenticatedUserDto user);
}

public sealed class PostLoginRedirectResolver : IPostLoginRedirectResolver
{
    public (string Controller, string Action) Resolve(AuthenticatedUserDto user)
    {
        var permissions = new HashSet<string>(user.Permisos, StringComparer.OrdinalIgnoreCase);

        if (permissions.Contains(SystemPermissions.UsuariosConsultar))
            return ("Usuarios", "Index");

        if (permissions.Contains(SystemPermissions.RolesConsultar))
            return ("Roles", "Index");

        return ("Home", "Index");
    }
}
