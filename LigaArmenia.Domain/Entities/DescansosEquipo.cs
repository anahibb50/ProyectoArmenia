using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class DescansosEquipo
{
    public long Id { get; set; }

    public long EquipoId { get; set; }

    public DateOnly FechaSabado { get; set; }

    public string Motivo { get; set; } = null!;

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Equipo Equipo { get; set; } = null!;
}
