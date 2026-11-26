using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class ArchivosVocalium
{
    public Guid UuidSync { get; set; }

    public long Id { get; set; }

    public long VocaliaId { get; set; }

    public string Tipo { get; set; } = null!;

    public string RutaArchivo { get; set; } = null!;

    public DateTime RegistradoEn { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Vocalias Vocalia { get; set; } = null!;
}
