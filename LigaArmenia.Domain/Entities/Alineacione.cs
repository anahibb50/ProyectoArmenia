using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Alineacione
{
    public Guid UuidSync { get; set; }

    public long Id { get; set; }

    public long VocaliaId { get; set; }

    public long EquipoId { get; set; }

    public long JugadorEquipoId { get; set; }

    public short NumeroCamiseta { get; set; }

    public bool Titular { get; set; }

    public bool Capitan { get; set; }

    public bool Participo { get; set; }

    public bool PresentoCarnet { get; set; }

    public short? MinutoEntrada { get; set; }

    public short? MinutoSalida { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Equipo Equipo { get; set; } = null!;

    public virtual JugadoresEquipo JugadoresEquipo { get; set; } = null!;

    public virtual Vocalias Vocalia { get; set; } = null!;
}
