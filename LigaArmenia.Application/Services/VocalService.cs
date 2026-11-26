using LigaArmenia.Application.DTOs.Common;
using LigaArmenia.Application.DTOs.Vocales;
using LigaArmenia.Application.Interfaces.Persistence;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Domain.Constants;

namespace LigaArmenia.Application.Services;

public sealed class VocalService : IVocalService
{
    private readonly IVocalRepository _vocalRepository;
    private readonly ITransactionRunner _transactionRunner;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserSecurityStateService _securityStateService;

    public VocalService(
        IVocalRepository vocalRepository,
        ITransactionRunner transactionRunner,
        ICurrentUserService currentUser,
        IUserSecurityStateService securityStateService)
    {
        _vocalRepository = vocalRepository;
        _transactionRunner = transactionRunner;
        _currentUser = currentUser;
        _securityStateService = securityStateService;
    }

    public async Task<VocalPagedResultDto> SearchPagedAsync(VocalFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = Math.Clamp(filter.PageSize, 5, 50);

        var (items, totalFiltered, totalRegistrados, totalActivos, totalInactivos) =
            await _vocalRepository.SearchPagedAsync(filter.Busqueda, filter.Activo, page, pageSize, cancellationToken);

        var mapped = items.Select(v => new VocalListItemDto
        {
            Id = v.Id,
            UsuarioId = v.UsuarioId,
            Nombres = v.Nombres,
            Correo = v.Usuario?.Correo,
            Telefono = v.Telefono,
            Activo = v.Activo,
            UsuarioActivo = v.Usuario?.Activo == true,
            TieneRolVocal = v.Usuario?.UsuariosRoleUsuarios.Any(ur =>
                string.Equals(ur.Rol.Nombre, SystemRoles.Vocal, StringComparison.OrdinalIgnoreCase)) == true,
            CreadoEn = v.CreadoEn,
            ActualizadoEn = v.ActualizadoEn
        }).ToList();

        var totalPages = Math.Max(1, (int)Math.Ceiling((double)totalFiltered / pageSize));
        var safePage = Math.Clamp(page, 1, totalPages);

        return new VocalPagedResultDto
        {
            Items = mapped,
            Page = safePage,
            PageSize = pageSize,
            TotalFiltered = totalFiltered,
            TotalRegistrados = totalRegistrados,
            TotalActivos = totalActivos,
            TotalInactivos = totalInactivos
        };
    }

    public async Task<VocalDetailDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var vocal = await _vocalRepository.GetByIdWithUserAsync(id, cancellationToken);
        if (vocal is null)
            return null;

        return new VocalDetailDto
        {
            Id = vocal.Id,
            UsuarioId = vocal.UsuarioId,
            Nombres = vocal.Nombres,
            Correo = vocal.Usuario?.Correo,
            Telefono = vocal.Telefono,
            Activo = vocal.Activo,
            UsuarioActivo = vocal.Usuario?.Activo == true,
            TieneRolVocal = vocal.Usuario?.UsuariosRoleUsuarios.Any(ur =>
                string.Equals(ur.Rol.Nombre, SystemRoles.Vocal, StringComparison.OrdinalIgnoreCase)) == true,
            CreadoEn = vocal.CreadoEn,
            ActualizadoEn = vocal.ActualizadoEn
        };
    }

    public async Task<OperationResult> UpdateAsync(UpdateVocalDto dto, CancellationToken cancellationToken = default)
    {
        var nombres = dto.Nombres?.Trim() ?? string.Empty;
        var telefono = NormalizePhone(dto.Telefono);

        if (string.IsNullOrWhiteSpace(nombres))
            return OperationResult.Fail("Los nombres completos son obligatorios.");

        if (nombres.Length > 180)
            return OperationResult.Fail("Los nombres no pueden superar los 180 caracteres.");

        if (telefono is not null && telefono.Length > 35)
            return OperationResult.Fail("El teléfono no puede superar los 35 caracteres.");

        return await _transactionRunner.ExecuteAsync(async ct =>
        {
            var vocal = await _vocalRepository.GetByIdWithUserAsync(dto.Id, ct);
            if (vocal is null)
                return OperationResult.Fail("El perfil de vocal solicitado no existe.");

            var now = DateTime.UtcNow;
            var actorId = _currentUser.UserId;

            vocal.Nombres = nombres;
            vocal.Telefono = telefono;
            vocal.ActualizadoEn = now;
            vocal.ActualizadoPor = actorId;

            if (vocal.Usuario is not null)
            {
                vocal.Usuario.Nombres = nombres;
                vocal.Usuario.ActualizadoEn = now;
                vocal.Usuario.ActualizadoPor = actorId;
            }

            await _vocalRepository.SaveChangesAsync(ct);

            if (vocal.UsuarioId.HasValue)
                _securityStateService.InvalidateUser(vocal.UsuarioId.Value);

            return OperationResult.Ok("Información del vocal actualizada correctamente.");
        }, cancellationToken);
    }

    public async Task<OperationResult> SetActiveStatusAsync(long vocalId, bool activo, CancellationToken cancellationToken = default)
    {
        return await _transactionRunner.ExecuteAsync(async ct =>
        {
            var vocal = await _vocalRepository.GetByIdWithUserAsync(vocalId, ct);
            if (vocal is null)
                return OperationResult.Fail("El perfil de vocal solicitado no existe.");

            if (!activo)
            {
                if (!vocal.Activo)
                    return OperationResult.Ok("El vocal ya se encuentra inactivo.");

                var pendingMatches = await _vocalRepository.CountPendingAssignedMatchesAsync(vocal.Id, ct);
                if (pendingMatches > 0)
                {
                    return OperationResult.Fail(
                        $"No se puede desactivar al vocal porque tiene {pendingMatches} partido(s) pendiente(s) asignado(s). " +
                        "Reasigna primero esos partidos antes de desactivarlo.");
                }
            }
            else
            {
                if (vocal.Usuario is null)
                {
                    return OperationResult.Fail(
                        "No se puede activar este perfil de vocal porque no tiene una cuenta de usuario asociada.");
                }

                if (!vocal.Usuario.Activo)
                {
                    return OperationResult.Fail(
                        "No se puede activar el vocal porque su cuenta de usuario asociada se encuentra inactiva. " +
                        "Activa primero la cuenta desde el módulo de Usuarios.");
                }

                var hasVocalRole = vocal.Usuario.UsuariosRoleUsuarios.Any(ur =>
                    string.Equals(ur.Rol.Nombre, SystemRoles.Vocal, StringComparison.OrdinalIgnoreCase));

                if (!hasVocalRole)
                {
                    return OperationResult.Fail(
                        "No se puede activar el vocal porque su usuario ya no tiene asignado el rol VOCAL.");
                }
            }

            vocal.Activo = activo;
            vocal.ActualizadoEn = DateTime.UtcNow;
            vocal.ActualizadoPor = _currentUser.UserId;

            await _vocalRepository.SaveChangesAsync(ct);

            if (vocal.UsuarioId.HasValue)
                _securityStateService.InvalidateUser(vocal.UsuarioId.Value);

            return OperationResult.Ok(activo
                ? "El perfil de vocal ha sido activado correctamente."
                : "El perfil de vocal ha sido desactivado. Su cuenta de usuario y otros roles se mantienen sin cambios.");
        }, cancellationToken);
    }

    public Task<IReadOnlyList<VocalDisponibleDto>> GetAvailableForDesignationAsync(CancellationToken cancellationToken = default)
    {
        return _vocalRepository.GetAvailableForDesignationAsync(cancellationToken);
    }

    private static string? NormalizePhone(string? phone)
    {
        var trimmed = phone?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
