using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class CambiosProgramacion
{
    public long Id { get; set; }

    public long PartidoId { get; set; }

    public DateOnly? FechaAnterior { get; set; }

    public DateOnly? FechaNueva { get; set; }

    public short? HorarioAnteriorId { get; set; }

    public short? HorarioNuevoId { get; set; }

    public string Motivo { get; set; } = null!;

    public long? RealizadoPor { get; set; }

    public DateTime RegistradoEn { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Horario? HorarioAnterior { get; set; }

    public virtual Horario? HorarioNuevo { get; set; }

    public virtual Partido Partido { get; set; } = null!;

    public virtual Usuario? RealizadoPorNavigation { get; set; }
}
