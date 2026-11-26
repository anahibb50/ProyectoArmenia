using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Reunione
{
    public long Id { get; set; }

    public DateOnly Fecha { get; set; }

    public string? NumeroActa { get; set; }

    public string Tipo { get; set; } = null!;

    public string? Observaciones { get; set; }
}
