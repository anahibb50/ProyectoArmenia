namespace LigaArmenia.Application.DTOs.Users;

public sealed class UsuarioFilterDto
{
    public string? Busqueda { get; set; }
    public long? RolId { get; set; }
    public bool? Activo { get; set; }
}

public sealed class UsuarioListItemDto
{
    public long Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public bool Activo { get; init; }
    public DateTime CreadoEn { get; init; }
    public DateTime ActualizadoEn { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
}

public sealed class UsuarioDetailDto
{
    public long Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public bool Activo { get; init; }
    public DateTime CreadoEn { get; init; }
    public DateTime ActualizadoEn { get; init; }
    public IReadOnlyList<long> RoleIds { get; init; } = Array.Empty<long>();
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public bool TienePerfilVocal { get; init; }
    public bool VocalActivo { get; init; }
    public string? TelefonoVocal { get; init; }
}

public sealed class CreateUsuarioDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public List<long> RoleIds { get; set; } = new();
    public string? TelefonoVocal { get; set; }
}

public sealed class UpdateUsuarioDto
{
    public long Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public List<long> RoleIds { get; set; } = new();
    public string? TelefonoVocal { get; set; }
}

public sealed class ResetPasswordDto
{
    public long UsuarioId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}
