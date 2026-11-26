using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class VwCargosPendiente
{
    public long? CargoId { get; set; }

    public long? PartidoId { get; set; }

    public long? EquipoId { get; set; }

    public string? Equipo { get; set; }

    public string? Tipo { get; set; }

    public string? Concepto { get; set; }

    public decimal? Monto { get; set; }

    public decimal? Pagado { get; set; }

    public decimal? Saldo { get; set; }
}
