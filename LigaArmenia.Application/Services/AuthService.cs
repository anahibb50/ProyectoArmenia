using LigaArmenia.Application.DTOs.Auth;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Domain.Rules;

namespace LigaArmenia.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPermisoRepository _permisoRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ILoginAttemptTracker _attemptTracker;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPermisoRepository permisoRepository,
        IPasswordHasherService passwordHasher,
        ILoginAttemptTracker attemptTracker)
    {
        _usuarioRepository = usuarioRepository;
        _permisoRepository = permisoRepository;
        _passwordHasher = passwordHasher;
        _attemptTracker = attemptTracker;
    }

    public async Task<AuthResultDto> AuthenticateAsync(
        LoginRequestDto request,
        AuthChannel channel,
        string? clientIp,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = PasswordPolicy.NormalizeEmail(request.Correo);
        var attemptKey = BuildAttemptKey(normalizedEmail, clientIp);

        if (_attemptTracker.IsLockedOut(attemptKey, out var remaining))
            return AuthResultDto.LockedOut(remaining);

        if (string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(request.Password))
        {
            _attemptTracker.RecordFailure(attemptKey);
            return AuthResultDto.InvalidCredentials();
        }

        var usuario = await _usuarioRepository.GetByEmailWithRolesAndPermissionsAsync(normalizedEmail, cancellationToken);
        if (usuario is null || !usuario.Activo)
        {
            _attemptTracker.RecordFailure(attemptKey);
            return AuthResultDto.InvalidCredentials();
        }

        var validPassword = _passwordHasher.VerifyPassword(usuario, usuario.PasswordHash, request.Password, out var needsRehash);
        if (!validPassword)
        {
            _attemptTracker.RecordFailure(attemptKey);
            return AuthResultDto.InvalidCredentials();
        }

        var roles = usuario.UsuariosRoleUsuarios
            .Select(ur => ur.Rol.Nombre)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(r => r)
            .ToList();

        if (roles.Count == 0)
        {
            _attemptTracker.RecordFailure(attemptKey);
            return AuthResultDto.InvalidCredentials();
        }

        if (channel == AuthChannel.WebPortal)
        {
            var canUseWeb = roles.Any(r => SystemRoles.WebPortalAllowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
            if (!canUseWeb)
            {
                return AuthResultDto.ChannelForbidden(
                    "Tu cuenta corresponde al rol VOCAL y debe acceder exclusivamente mediante la aplicación móvil.");
            }
        }
        else if (channel == AuthChannel.MobileApi)
        {
            var canUseMobile = roles.Any(r => SystemRoles.MobileAllowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
            if (!canUseMobile)
            {
                return AuthResultDto.ChannelForbidden(
                    "Tu cuenta no cuenta con autorización para acceder desde la aplicación móvil.");
            }
        }

        if (needsRehash)
        {
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);
            usuario.ActualizadoEn = DateTime.UtcNow;
            await _usuarioRepository.SaveChangesAsync(cancellationToken);
        }

        _attemptTracker.Reset(attemptKey);

        var permissionsSet = new HashSet<string>(
            usuario.UsuariosRoleUsuarios
                .SelectMany(ur => ur.Rol.RolesPermisos)
                .Select(rp => rp.Permiso.Codigo),
            StringComparer.OrdinalIgnoreCase);

        if (roles.Contains(SystemRoles.Administrador, StringComparer.OrdinalIgnoreCase))
        {
            var allCodes = await _permisoRepository.GetAllCodesAsync(cancellationToken);
            foreach (var code in allCodes)
                permissionsSet.Add(code);
        }

        var dto = new AuthenticatedUserDto
        {
            Id = usuario.Id,
            Nombres = usuario.Nombres,
            Correo = usuario.Correo,
            VocalId = usuario.VocaleUsuario is { Activo: true } ? usuario.VocaleUsuario.Id : null,
            Roles = roles,
            Permisos = permissionsSet.OrderBy(p => p).ToList()
        };

        return AuthResultDto.Success(dto);
    }

    private static string BuildAttemptKey(string normalizedEmail, string? clientIp)
    {
        var ip = string.IsNullOrWhiteSpace(clientIp) ? "unknown" : clientIp.Trim();
        return $"{normalizedEmail}|{ip}";
    }
}
