using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class VwGoleadore
{
    public long? TemporadaId { get; set; }

    public char? Serie { get; set; }

    public long? EquipoId { get; set; }

    public string? Equipo { get; set; }

    public long? JugadorId { get; set; }

    public string? Jugador { get; set; }

    public long? Goles { get; set; }
}
