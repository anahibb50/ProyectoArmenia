using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class PagosCargo
{
    public long Id { get; set; }

    public long CargoId { get; set; }

    public decimal Monto { get; set; }

    public DateTime PagadoEn { get; set; }

    public string? Referencia { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual CargosPartido Cargo { get; set; } = null!;

    public virtual Usuario? CreadoPorNavigation { get; set; }
}
