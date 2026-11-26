using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class UsuariosRole
{
    public long UsuarioId { get; set; }

    public long RolId { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Role Rol { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
