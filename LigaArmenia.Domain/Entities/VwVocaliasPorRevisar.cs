using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class VwVocaliasPorRevisar
{
    public long? PartidoId { get; set; }

    public DateOnly? Fecha { get; set; }

    public char? Serie { get; set; }

    public short? Etapa { get; set; }

    public string? EquipoLocal { get; set; }

    public string? EquipoVisitante { get; set; }

    public long? VocaliaId { get; set; }

    public string? EstadoVocalia { get; set; }

    public long? EquipoResponsableId { get; set; }

    public long? VocalId { get; set; }

    public string? VocalDesignado { get; set; }
}
