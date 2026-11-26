using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Equipo
{
    public long Id { get; set; }

    public long TemporadaId { get; set; }

    public string Nombre { get; set; } = null!;

    public char Serie { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual ICollection<Alineacione> Alineaciones { get; set; } = new List<Alineacione>();

    public virtual ICollection<CambiosDesignacionVocal> CambiosDesignacionVocalEquipoAnteriors { get; set; } = new List<CambiosDesignacionVocal>();

    public virtual ICollection<CambiosDesignacionVocal> CambiosDesignacionVocalEquipoNuevos { get; set; } = new List<CambiosDesignacionVocal>();

    public virtual ICollection<CargosPartido> CargosPartidos { get; set; } = new List<CargosPartido>();

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual ICollection<DescansosEquipo> DescansosEquipos { get; set; } = new List<DescansosEquipo>();

    public virtual ICollection<DesignacionesVocalium> DesignacionesVocalia { get; set; } = new List<DesignacionesVocalium>();

    public virtual ICollection<EventosPartido> EventosPartidos { get; set; } = new List<EventosPartido>();

    public virtual ICollection<JugadoresEquipo> JugadoresEquipos { get; set; } = new List<JugadoresEquipo>();

    public virtual ICollection<Partido> PartidoEquipoNavigations { get; set; } = new List<Partido>();

    public virtual ICollection<Partido> PartidoEquipos { get; set; } = new List<Partido>();

    public virtual Temporada Temporada { get; set; } = null!;
}
