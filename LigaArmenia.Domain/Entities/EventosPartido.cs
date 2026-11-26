using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class EventosPartido
{
    public Guid UuidSync { get; set; }

    public long Id { get; set; }

    public long VocaliaId { get; set; }

    public long EquipoId { get; set; }

    public long? JugadorEquipoId { get; set; }

    public long? JugadorRelacionadoId { get; set; }

    public string Tipo { get; set; } = null!;

    public short? Minuto { get; set; }

    public string? Motivo { get; set; }

    public string? Observaciones { get; set; }

    public bool Valido { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual ICollection<CargosPartido> CargosPartidos { get; set; } = new List<CargosPartido>();

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Equipo Equipo { get; set; } = null!;

    public virtual JugadoresEquipo? JugadoresEquipo { get; set; }

    public virtual JugadoresEquipo? JugadoresEquipoNavigation { get; set; }

    public virtual Vocalias Vocalia { get; set; } = null!;
}
