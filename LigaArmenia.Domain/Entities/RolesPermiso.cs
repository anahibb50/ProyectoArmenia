using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class RolesPermiso
{
    public long RolId { get; set; }

    public long PermisoId { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual Permiso Permiso { get; set; } = null!;

    public virtual Role Rol { get; set; } = null!;
}
