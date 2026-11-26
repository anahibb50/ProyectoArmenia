using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class JugadoresEquipo
{
    public long Id { get; set; }

    public long JugadorId { get; set; }

    public long EquipoId { get; set; }

    public DateOnly FechaAlta { get; set; }

    public DateOnly? FechaBaja { get; set; }

    public bool Habilitado { get; set; }

    public bool ConsideradoJuvenil { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual ICollection<Alineacione> Alineaciones { get; set; } = new List<Alineacione>();

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Equipo Equipo { get; set; } = null!;

    public virtual ICollection<EventosPartido> EventosPartidoJugadoresEquipoNavigations { get; set; } = new List<EventosPartido>();

    public virtual ICollection<EventosPartido> EventosPartidoJugadoresEquipos { get; set; } = new List<EventosPartido>();

    public virtual Jugadore Jugador { get; set; } = null!;
}
