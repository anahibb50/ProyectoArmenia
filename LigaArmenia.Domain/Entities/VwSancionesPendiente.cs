using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class VwSancionesPendiente
{
    public long? SancionId { get; set; }

    public long? JugadorId { get; set; }

    public string? Jugador { get; set; }

    public string? Motivo { get; set; }

    public string? Estado { get; set; }

    public short? PartidosImpuestos { get; set; }

    public long? PartidosCumplidos { get; set; }

    public long? PartidosPendientes { get; set; }
}
