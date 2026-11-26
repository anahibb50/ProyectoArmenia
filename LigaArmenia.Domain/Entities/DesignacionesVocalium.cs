using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class DesignacionesVocalium
{
    public long Id { get; set; }

    public long PartidoId { get; set; }

    public long EquipoResponsableId { get; set; }

    public long? VocalId { get; set; }

    public DateTime DesignadoEn { get; set; }

    public string? Observaciones { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Equipo EquipoResponsable { get; set; } = null!;

    public virtual Partido Partido { get; set; } = null!;

    public virtual Vocale? Vocal { get; set; }
}
