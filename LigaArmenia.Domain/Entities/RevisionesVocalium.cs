using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class RevisionesVocalium
{
    public long Id { get; set; }

    public long VocaliaId { get; set; }

    public long ReunionId { get; set; }

    public string Decision { get; set; } = null!;

    public short? GolesLocalResueltos { get; set; }

    public short? GolesVisitanteResueltos { get; set; }

    public string? TipoResultado { get; set; }

    public string? Fundamento { get; set; }

    public DateTime RevisadaEn { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Vocalias Vocalia { get; set; } = null!;
}
