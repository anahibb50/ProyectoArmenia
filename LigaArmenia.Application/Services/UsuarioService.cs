using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.DTOs.Users;
using LigaArmenia.Application.Interfaces.Persistence;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Domain.Rules;

namespace LigaArmenia.Application.Services;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IVocalRepository _vocalRepository;
    private readonly ITransactionRunner _transactionRunner;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserSecurityStateService _securityStateService;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IVocalRepository vocalRepository,
        ITransactionRunner transactionRunner,
        IPasswordHasherService passwordHasher,
        ICurrentUserService currentUser,
        IUserSecurityStateService securityStateService)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _vocalRepository = vocalRepository;
        _transactionRunner = transactionRunner;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _securityStateService = securityStateService;
    }

    public async Task<IReadOnlyList<UsuarioListItemDto>> SearchAsync(UsuarioFilterDto filter, CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.SearchAsync(filter.Busqueda, filter.RolId, filter.Activo, cancellationToken);
        return usuarios.Select(u => new UsuarioListItemDto
        {
            Id = u.Id,
            Nombres = u.Nombres,
            Correo = u.Correo,
            Activo = u.Activo,
            CreadoEn = u.CreadoEn,
            ActualizadoEn = u.ActualizadoEn,
            Roles = u.UsuariosRoleUsuarios
                .Select(ur => ur.Rol.Nombre)
                .OrderBy(r => r)
                .ToList()
        }).ToList();
    }

    public async Task<UsuarioDetailDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdWithRolesAsync(id, cancellationToken);
        if (usuario is null)
            return null;

        return new UsuarioDetailDto
        {
            Id = usuario.Id,
            Nombres = usuario.Nombres,
            Correo = usuario.Correo,
            Activo = usuario.Activo,
            CreadoEn = usuario.CreadoEn,
            ActualizadoEn = usuario.ActualizadoEn,
            RoleIds = usuario.UsuariosRoleUsuarios.Select(ur => ur.RolId).ToList(),
            Roles = usuario.UsuariosRoleUsuarios.Select(ur => ur.Rol.Nombre).OrderBy(r => r).ToList(),
            TienePerfilVocal = usuario.VocaleUsuario is not null,
            VocalActivo = usuario.VocaleUsuario?.Activo == true,
            TelefonoVocal = usuario.VocaleUsuario?.Telefono
        };
    }

    public async Task<OperationResult<long>> CreateAsync(CreateUsuarioDto dto, CancellationToken cancellationToken = default)
    {
        var nombres = dto.Nombres?.Trim() ?? string.Empty;
        var correo = PasswordPolicy.NormalizeEmail(dto.Correo);
        var telefonoVocal = NormalizePhone(dto.TelefonoVocal);

        if (string.IsNullOrWhiteSpace(nombres))
            return OperationResult<long>.Fail("Los nombres son obligatorios.");

        if (nombres.Length > 180)
            return OperationResult<long>.Fail("Los nombres no pueden superar los 180 caracteres.");

        if (string.IsNullOrWhiteSpace(correo) || correo.Length > 254 || !correo.Contains('@'))
            return OperationResult<long>.Fail("Ingresa un correo electrónico válido.");

        if (telefonoVocal is not null && telefonoVocal.Length > 35)
            return OperationResult<long>.Fail("El teléfono del vocal no puede superar los 35 caracteres.");

        var (isValidPassword, passwordErrors) = PasswordPolicy.Validate(dto.Password);
        if (!isValidPassword)
            return OperationResult<long>.Fail(passwordErrors);

        var distinctRoleIds = dto.RoleIds.Distinct().ToList();
        if (distinctRoleIds.Count == 0)
            return OperationResult<long>.Fail("Debes asignar al menos un rol al usuario.");

        if (await _usuarioRepository.ExistsByEmailAsync(correo, cancellationToken: cancellationToken))
            return OperationResult<long>.Fail("Ya existe un usuario registrado con ese correo electrónico.");

        var validRoles = await _rolRepository.GetByIdsAsync(distinctRoleIds, cancellationToken);
        if (validRoles.Count != distinctRoleIds.Count)
            return OperationResult<long>.Fail("Uno o más roles seleccionados no son válidos.");

        var hasVocalRole = validRoles.Any(r =>
            string.Equals(r.Nombre, SystemRoles.Vocal, StringComparison.OrdinalIgnoreCase));

        return await _transactionRunner.ExecuteAsync(async ct =>
        {
            var now = DateTime.UtcNow;
            var actorId = _currentUser.UserId;

            var usuario = new Usuario
            {
                Nombres = nombres,
                Correo = correo,
                Activo = dto.Activo,
                CreadoEn = now,
                ActualizadoEn = now,
                CreadoPor = actorId,
                ActualizadoPor = actorId
            };

            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, dto.Password);

            await _usuarioRepository.AddAsync(usuario, ct);
            await _usuarioRepository.SaveChangesAsync(ct);

            await _usuarioRepository.ReplaceUserRolesAsync(usuario.Id, distinctRoleIds, actorId, now, ct);

            if (hasVocalRole)
            {
                var vocal = new Vocale
                {
                    UsuarioId = usuario.Id,
                    Nombres = nombres,
                    Telefono = telefonoVocal,
                    Activo = dto.Activo,
                    CreadoEn = now,
                    ActualizadoEn = now,
                    CreadoPor = actorId,
                    ActualizadoPor = actorId
                };

                await _vocalRepository.AddAsync(vocal, ct);
            }

            await _usuarioRepository.SaveChangesAsync(ct);

            var successMessage = hasVocalRole
                ? "Usuario y perfil de vocal registrados correctamente."
                : "Usuario registrado correctamente.";

            return OperationResult<long>.Ok(usuario.Id, successMessage);
        }, cancellationToken);
    }

    public async Task<OperationResult> UpdateAsync(UpdateUsuarioDto dto, CancellationToken cancellationToken = default)
    {
        var nombres = dto.Nombres?.Trim() ?? string.Empty;
        var correo = PasswordPolicy.NormalizeEmail(dto.Correo);
        var telefonoVocal = NormalizePhone(dto.TelefonoVocal);

        if (string.IsNullOrWhiteSpace(nombres))
            return OperationResult.Fail("Los nombres son obligatorios.");

        if (nombres.Length > 180)
            return OperationResult.Fail("Los nombres no pueden superar los 180 caracteres.");

        if (string.IsNullOrWhiteSpace(correo) || correo.Length > 254 || !correo.Contains('@'))
            return OperationResult.Fail("Ingresa un correo electrónico válido.");

        if (telefonoVocal is not null && telefonoVocal.Length > 35)
            return OperationResult.Fail("El teléfono del vocal no puede superar los 35 caracteres.");

        var distinctRoleIds = dto.RoleIds.Distinct().ToList();
        if (distinctRoleIds.Count == 0)
            return OperationResult.Fail("El usuario debe mantener al menos un rol asignado.");

        var validRoles = await _rolRepository.GetByIdsAsync(distinctRoleIds, cancellationToken);
        if (validRoles.Count != distinctRoleIds.Count)
            return OperationResult.Fail("Uno o más roles seleccionados no son válidos.");

        return await _transactionRunner.ExecuteAsync(async ct =>
        {
            var usuario = await _usuarioRepository.GetByIdWithRolesAsync(dto.Id, ct);
            if (usuario is null)
                return OperationResult.Fail("El usuario especificado no existe.");

            if (await _usuarioRepository.ExistsByEmailAsync(correo, excludeUserId: usuario.Id, ct))
                return OperationResult.Fail("Ya existe otro usuario registrado con ese correo electrónico.");

            var currentlyAdmin = usuario.UsuariosRoleUsuarios
                .Any(ur => string.Equals(ur.Rol.Nombre, SystemRoles.Administrador, StringComparison.OrdinalIgnoreCase));

            var willBeAdmin = validRoles
                .Any(r => string.Equals(r.Nombre, SystemRoles.Administrador, StringComparison.OrdinalIgnoreCase));

            if (currentlyAdmin && !willBeAdmin && usuario.Activo)
            {
                var otherActiveAdmins = await _usuarioRepository.CountActiveUsersInRoleAsync(
                    SystemRoles.Administrador,
                    excludeUserId: usuario.Id,
                    ct);

                if (otherActiveAdmins == 0)
                    return OperationResult.Fail("No se puede retirar el rol ADMINISTRADOR del último administrador activo del sistema.");
            }

            var currentlyVocal = usuario.UsuariosRoleUsuarios
                .Any(ur => string.Equals(ur.Rol.Nombre, SystemRoles.Vocal, StringComparison.OrdinalIgnoreCase));

            var willBeVocal = validRoles
                .Any(r => string.Equals(r.Nombre, SystemRoles.Vocal, StringComparison.OrdinalIgnoreCase));

            var existingVocal = await _vocalRepository.GetByUsuarioIdAsync(usuario.Id, ct);

            if (currentlyVocal && !willBeVocal && existingVocal is { Activo: true })
            {
                var pendingMatches = await _vocalRepository.CountPendingAssignedMatchesAsync(existingVocal.Id, ct);
                if (pendingMatches > 0)
                {
                    return OperationResult.Fail(
                        $"No se puede retirar el rol VOCAL porque el usuario tiene {pendingMatches} partido(s) pendiente(s) asignado(s). " +
                        "Reasigna primero esos partidos.");
                }
            }

            var now = DateTime.UtcNow;
            var actorId = _currentUser.UserId;

            usuario.Nombres = nombres;
            usuario.Correo = correo;
            usuario.ActualizadoEn = now;
            usuario.ActualizadoPor = actorId;

            await _usuarioRepository.ReplaceUserRolesAsync(usuario.Id, distinctRoleIds, actorId, now, ct);

            if (willBeVocal)
            {
                if (existingVocal is null)
                {
                    await _vocalRepository.AddAsync(new Vocale
                    {
                        UsuarioId = usuario.Id,
                        Nombres = nombres,
                        Telefono = telefonoVocal,
                        Activo = usuario.Activo,
                        CreadoEn = now,
                        ActualizadoEn = now,
                        CreadoPor = actorId,
                        ActualizadoPor = actorId
                    }, ct);
                }
                else
                {
                    existingVocal.Nombres = nombres;
                    existingVocal.Telefono = telefonoVocal;
                    if (!currentlyVocal && usuario.Activo)
                        existingVocal.Activo = true;
                    existingVocal.ActualizadoEn = now;
                    existingVocal.ActualizadoPor = actorId;
                }
            }
            else if (existingVocal is not null)
            {
                existingVocal.Nombres = nombres;
                existingVocal.Activo = false;
                existingVocal.ActualizadoEn = now;
                existingVocal.ActualizadoPor = actorId;
            }

            await _usuarioRepository.SaveChangesAsync(ct);

            _securityStateService.InvalidateUser(usuario.Id);
            return OperationResult.Ok("Usuario actualizado correctamente.");
        }, cancellationToken);
    }

    public async Task<OperationResult> SetActiveStatusAsync(long usuarioId, bool activo, CancellationToken cancellationToken = default)
    {
        return await _transactionRunner.ExecuteAsync(async ct =>
        {
            var usuario = await _usuarioRepository.GetByIdWithRolesAsync(usuarioId, ct);
            if (usuario is null)
                return OperationResult.Fail("El usuario especificado no existe.");

            if (!activo && usuario.Activo)
            {
                var isAdmin = usuario.UsuariosRoleUsuarios
                    .Any(ur => string.Equals(ur.Rol.Nombre, SystemRoles.Administrador, StringComparison.OrdinalIgnoreCase));

                if (isAdmin)
                {
                    var otherActiveAdmins = await _usuarioRepository.CountActiveUsersInRoleAsync(
                        SystemRoles.Administrador,
                        excludeUserId: usuario.Id,
                        ct);

                    if (otherActiveAdmins == 0)
                        return OperationResult.Fail("No se puede desactivar al último administrador activo del sistema.");
                }
            }

            var now = DateTime.UtcNow;
            var actorId = _currentUser.UserId;

            usuario.Activo = activo;
            usuario.ActualizadoEn = now;
            usuario.ActualizadoPor = actorId;

            // Al desactivar el usuario también se desactiva su perfil de vocal.
            // Al reactivar el usuario NO se reactiva automáticamente un perfil de vocal desactivado.
            if (!activo)
            {
                var existingVocal = await _vocalRepository.GetByUsuarioIdAsync(usuario.Id, ct);
                if (existingVocal is { Activo: true })
                {
                    existingVocal.Activo = false;
                    existingVocal.ActualizadoEn = now;
                    existingVocal.ActualizadoPor = actorId;
                }
            }

            await _usuarioRepository.SaveChangesAsync(ct);
            _securityStateService.InvalidateUser(usuario.Id);

            return OperationResult.Ok(activo
                ? "La cuenta ha sido activada correctamente."
                : "La cuenta ha sido desactivada correctamente.");
        }, cancellationToken);
    }

    public async Task<OperationResult> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId, cancellationToken);
        if (usuario is null)
            return OperationResult.Fail("El usuario especificado no existe.");

        var (isValidPassword, passwordErrors) = PasswordPolicy.Validate(dto.NewPassword);
        if (!isValidPassword)
            return OperationResult.Fail(passwordErrors);

        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, dto.NewPassword);
        usuario.ActualizadoEn = DateTime.UtcNow;
        usuario.ActualizadoPor = _currentUser.UserId;

        await _usuarioRepository.SaveChangesAsync(cancellationToken);
        _securityStateService.InvalidateUser(usuario.Id);

        return OperationResult.Ok("La contraseña ha sido restablecida correctamente.");
    }

    private static string? NormalizePhone(string? phone)
    {
        var trimmed = phone?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
