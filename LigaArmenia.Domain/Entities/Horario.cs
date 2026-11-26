using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Horario
{
    public short Id { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual ICollection<CambiosProgramacion> CambiosProgramacionHorarioAnteriors { get; set; } = new List<CambiosProgramacion>();

    public virtual ICollection<CambiosProgramacion> CambiosProgramacionHorarioNuevos { get; set; } = new List<CambiosProgramacion>();

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual ICollection<Partido> Partidos { get; set; } = new List<Partido>();
}
