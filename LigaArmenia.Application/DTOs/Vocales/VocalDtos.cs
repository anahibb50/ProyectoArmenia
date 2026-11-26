namespace LigaArmenia.Application.DTOs.Vocales;

public sealed class VocalFilterDto
{
    public string? Busqueda { get; set; }
    public bool? Activo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public sealed class VocalListItemDto
{
    public long Id { get; init; }
    public long? UsuarioId { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string? Correo { get; init; }
    public string? Telefono { get; init; }
    public bool Activo { get; init; }
    public bool UsuarioActivo { get; init; }
    public bool TieneRolVocal { get; init; }
    public DateTime CreadoEn { get; init; }
    public DateTime ActualizadoEn { get; init; }
}

public sealed class VocalPagedResultDto
{
    public IReadOnlyList<VocalListItemDto> Items { get; init; } = Array.Empty<VocalListItemDto>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalFiltered { get; init; }
    public int TotalPages => PageSize <= 0 ? 1 : Math.Max(1, (int)Math.Ceiling((double)TotalFiltered / PageSize));
    public int TotalRegistrados { get; init; }
    public int TotalActivos { get; init; }
    public int TotalInactivos { get; init; }
}

public sealed class VocalDetailDto
{
    public long Id { get; init; }
    public long? UsuarioId { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string? Correo { get; init; }
    public string? Telefono { get; init; }
    public bool Activo { get; init; }
    public bool UsuarioActivo { get; init; }
    public bool TieneRolVocal { get; init; }
    public DateTime CreadoEn { get; init; }
    public DateTime ActualizadoEn { get; init; }
}

public sealed class UpdateVocalDto
{
    public long Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}

/// <summary>
/// DTO mínimo reutilizable para la selección de vocales disponibles
/// en futuras designaciones de partidos.
/// </summary>
public sealed class VocalDisponibleDto
{
    public long Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
}
