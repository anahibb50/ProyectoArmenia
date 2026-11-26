using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class VwTablaPosicione
{
    public long? TemporadaId { get; set; }

    public char? Serie { get; set; }

    public short? Etapa { get; set; }

    public string? Fase { get; set; }

    public string? GrupoFase { get; set; }

    public long? EquipoId { get; set; }

    public string? Equipo { get; set; }

    public long? Pj { get; set; }

    public long? Pg { get; set; }

    public long? Pe { get; set; }

    public long? Pp { get; set; }

    public long? Gf { get; set; }

    public long? Gc { get; set; }

    public long? Dg { get; set; }

    public long? Pts { get; set; }
}
