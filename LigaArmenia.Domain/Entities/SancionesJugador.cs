using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class SancionesJugador
{
    public long Id { get; set; }

    public long JugadorId { get; set; }

    public long? VocaliaOrigenId { get; set; }

    public long? ReunionId { get; set; }

    public string? Articulo { get; set; }

    public string Motivo { get; set; } = null!;

    public short PartidosImpuestos { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? ResueltaEn { get; set; }

    public string? Observaciones { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Jugadore Jugador { get; set; } = null!;

    public virtual ICollection<PartidosSancionCumplido> PartidosSancionCumplidos { get; set; } = new List<PartidosSancionCumplido>();

    public virtual Vocalias? VocaliaOrigen { get; set; }
}
