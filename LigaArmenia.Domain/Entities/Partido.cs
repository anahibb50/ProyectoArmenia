using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Partido
{
    public long Id { get; set; }

    public long TemporadaId { get; set; }

    public short CategoriaId { get; set; }

    public char? Serie { get; set; }

    public short Etapa { get; set; }

    public string Fase { get; set; } = null!;

    public string? GrupoFase { get; set; }

    public long EquipoLocalId { get; set; }

    public long EquipoVisitanteId { get; set; }

    public DateOnly? Fecha { get; set; }

    public short? HorarioId { get; set; }

    public string Estado { get; set; } = null!;

    public decimal CostoArbitrajeTotal { get; set; }

    public short? GolesLocalOficial { get; set; }

    public short? GolesVisitanteOficial { get; set; }

    public string? TipoResultado { get; set; }

    public string? MotivoResultado { get; set; }

    public DateTime? AprobadoEn { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual ICollection<CambiosDesignacionVocal> CambiosDesignacionVocals { get; set; } = new List<CambiosDesignacionVocal>();

    public virtual ICollection<CambiosProgramacion> CambiosProgramacions { get; set; } = new List<CambiosProgramacion>();

    public virtual ICollection<CargosPartido> CargosPartidos { get; set; } = new List<CargosPartido>();

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual DesignacionesVocalium? DesignacionesVocalium { get; set; }

    public virtual Equipo Equipo { get; set; } = null!;

    public virtual Equipo EquipoNavigation { get; set; } = null!;

    public virtual Horario? Horario { get; set; }

    public virtual ICollection<PartidosSancionCumplido> PartidosSancionCumplidos { get; set; } = new List<PartidosSancionCumplido>();

    public virtual Temporada Temporada { get; set; } = null!;

    public virtual Vocalias? Vocalias { get; set; }
}
