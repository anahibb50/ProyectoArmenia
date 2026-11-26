using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class CargosPartido
{
    public long Id { get; set; }

    public long PartidoId { get; set; }

    public long EquipoId { get; set; }

    public string Tipo { get; set; } = null!;

    public string Concepto { get; set; } = null!;

    public decimal Monto { get; set; }

    public string Estado { get; set; } = null!;

    public long? EventoId { get; set; }

    public long? ReunionId { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Equipo Equipo { get; set; } = null!;

    public virtual EventosPartido? Evento { get; set; }

    public virtual ICollection<PagosCargo> PagosCargos { get; set; } = new List<PagosCargo>();

    public virtual Partido Partido { get; set; } = null!;
}
