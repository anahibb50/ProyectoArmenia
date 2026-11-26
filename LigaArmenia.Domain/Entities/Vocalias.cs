using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Vocalias
{
    public int Version { get; set; }

    public Guid UuidSync { get; set; }

    public long Id { get; set; }

    public long PartidoId { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? InicioReal { get; set; }

    public DateTime? FinReal { get; set; }

    public string? ArbitroPrincipal { get; set; }

    public string? ArbitroLinea1 { get; set; }

    public string? ArbitroLinea2 { get; set; }

    public short? GolesLocalCancha { get; set; }

    public short? GolesVisitanteCancha { get; set; }

    public string? InformeArbitro { get; set; }

    public string? InformeVocal { get; set; }

    public string? ObservacionesCapitanLocal { get; set; }

    public string? ObservacionesCapitanVisitante { get; set; }

    public string? IncidentesGenerales { get; set; }

    public string? PagosDeudasNotas { get; set; }

    public bool NoHizoVocalia { get; set; }

    public bool MalaVocalia { get; set; }

    public bool NoPresentoLocal { get; set; }

    public bool NoPresentoVisitante { get; set; }

    public bool FirmaCapitanLocal { get; set; }

    public bool FirmaCapitanVisitante { get; set; }

    public bool FirmaArbitro { get; set; }

    public bool FirmaVocal { get; set; }

    public DateTime? CerradaEn { get; set; }

    public DateTime? EnviadaRevisionEn { get; set; }

    public DateTime? AprobadaEn { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual ICollection<Alineacione> Alineaciones { get; set; } = new List<Alineacione>();

    public virtual ICollection<ArchivosVocalium> ArchivosVocalia { get; set; } = new List<ArchivosVocalium>();

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual ICollection<EventosPartido> EventosPartidos { get; set; } = new List<EventosPartido>();

    public virtual Partido Partido { get; set; } = null!;

    public virtual ICollection<RevisionesVocalium> RevisionesVocalia { get; set; } = new List<RevisionesVocalium>();

    public virtual ICollection<SancionesJugador> SancionesJugadors { get; set; } = new List<SancionesJugador>();
}
