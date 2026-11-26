using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class PartidosSancionCumplido
{
    public long Id { get; set; }

    public long SancionId { get; set; }

    public long PartidoId { get; set; }

    public DateTime RegistradoEn { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Partido Partido { get; set; } = null!;

    public virtual SancionesJugador Sancion { get; set; } = null!;
}
