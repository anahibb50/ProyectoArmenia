using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Jugadore
{
    public long Id { get; set; }

    public string Documento { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public DateOnly? FechaNacimiento { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual ICollection<JugadoresEquipo> JugadoresEquipos { get; set; } = new List<JugadoresEquipo>();

    public virtual ICollection<SancionesJugador> SancionesJugadors { get; set; } = new List<SancionesJugador>();
}
