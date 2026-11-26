using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class CambiosDesignacionVocal
{
    public long Id { get; set; }

    public long PartidoId { get; set; }

    public long? EquipoAnteriorId { get; set; }

    public long? EquipoNuevoId { get; set; }

    public long? VocalAnteriorId { get; set; }

    public long? VocalNuevoId { get; set; }

    public string Motivo { get; set; } = null!;

    public long? CambiadoPor { get; set; }

    public DateTime RegistradoEn { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CambiadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Equipo? EquipoAnterior { get; set; }

    public virtual Equipo? EquipoNuevo { get; set; }

    public virtual Partido Partido { get; set; } = null!;

    public virtual Vocale? VocalAnterior { get; set; }

    public virtual Vocale? VocalNuevo { get; set; }
}
