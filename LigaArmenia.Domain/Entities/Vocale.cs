using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Vocale
{
    public long? UsuarioId { get; set; }

    public long Id { get; set; }

    public string Nombres { get; set; } = null!;

    public string? Telefono { get; set; }

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual ICollection<CambiosDesignacionVocal> CambiosDesignacionVocalVocalAnteriors { get; set; } = new List<CambiosDesignacionVocal>();

    public virtual ICollection<CambiosDesignacionVocal> CambiosDesignacionVocalVocalNuevos { get; set; } = new List<CambiosDesignacionVocal>();

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual ICollection<DesignacionesVocalium> DesignacionesVocalia { get; set; } = new List<DesignacionesVocalium>();

    public virtual Usuario? Usuario { get; set; }
}
