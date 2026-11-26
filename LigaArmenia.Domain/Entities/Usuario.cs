using System;
using System.Collections.Generic;

namespace LigaArmenia.Domain.Entities;

public partial class Usuario
{
    public long Id { get; set; }

    public string Nombres { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime CreadoEn { get; set; }

    public DateTime ActualizadoEn { get; set; }

    public long? CreadoPor { get; set; }

    public long? ActualizadoPor { get; set; }

    public virtual Usuario? ActualizadoPorNavigation { get; set; }

    public virtual ICollection<Alineacione> AlineacioneActualizadoPorNavigations { get; set; } = new List<Alineacione>();

    public virtual ICollection<Alineacione> AlineacioneCreadoPorNavigations { get; set; } = new List<Alineacione>();

    public virtual ICollection<ArchivosVocalium> ArchivosVocaliumActualizadoPorNavigations { get; set; } = new List<ArchivosVocalium>();

    public virtual ICollection<ArchivosVocalium> ArchivosVocaliumCreadoPorNavigations { get; set; } = new List<ArchivosVocalium>();

    public virtual ICollection<CambiosDesignacionVocal> CambiosDesignacionVocalActualizadoPorNavigations { get; set; } = new List<CambiosDesignacionVocal>();

    public virtual ICollection<CambiosDesignacionVocal> CambiosDesignacionVocalCambiadoPorNavigations { get; set; } = new List<CambiosDesignacionVocal>();

    public virtual ICollection<CambiosDesignacionVocal> CambiosDesignacionVocalCreadoPorNavigations { get; set; } = new List<CambiosDesignacionVocal>();

    public virtual ICollection<CambiosProgramacion> CambiosProgramacionActualizadoPorNavigations { get; set; } = new List<CambiosProgramacion>();

    public virtual ICollection<CambiosProgramacion> CambiosProgramacionCreadoPorNavigations { get; set; } = new List<CambiosProgramacion>();

    public virtual ICollection<CambiosProgramacion> CambiosProgramacionRealizadoPorNavigations { get; set; } = new List<CambiosProgramacion>();

    public virtual ICollection<CargosPartido> CargosPartidoActualizadoPorNavigations { get; set; } = new List<CargosPartido>();

    public virtual ICollection<CargosPartido> CargosPartidoCreadoPorNavigations { get; set; } = new List<CargosPartido>();

    public virtual Usuario? CreadoPorNavigation { get; set; }

    public virtual ICollection<DescansosEquipo> DescansosEquipoActualizadoPorNavigations { get; set; } = new List<DescansosEquipo>();

    public virtual ICollection<DescansosEquipo> DescansosEquipoCreadoPorNavigations { get; set; } = new List<DescansosEquipo>();

    public virtual ICollection<DesignacionesVocalium> DesignacionesVocaliumActualizadoPorNavigations { get; set; } = new List<DesignacionesVocalium>();

    public virtual ICollection<DesignacionesVocalium> DesignacionesVocaliumCreadoPorNavigations { get; set; } = new List<DesignacionesVocalium>();

    public virtual ICollection<Equipo> EquipoActualizadoPorNavigations { get; set; } = new List<Equipo>();

    public virtual ICollection<Equipo> EquipoCreadoPorNavigations { get; set; } = new List<Equipo>();

    public virtual ICollection<EventosPartido> EventosPartidoActualizadoPorNavigations { get; set; } = new List<EventosPartido>();

    public virtual ICollection<EventosPartido> EventosPartidoCreadoPorNavigations { get; set; } = new List<EventosPartido>();

    public virtual ICollection<Horario> HorarioActualizadoPorNavigations { get; set; } = new List<Horario>();

    public virtual ICollection<Horario> HorarioCreadoPorNavigations { get; set; } = new List<Horario>();

    public virtual ICollection<Usuario> InverseActualizadoPorNavigation { get; set; } = new List<Usuario>();

    public virtual ICollection<Usuario> InverseCreadoPorNavigation { get; set; } = new List<Usuario>();

    public virtual ICollection<Jugadore> JugadoreActualizadoPorNavigations { get; set; } = new List<Jugadore>();

    public virtual ICollection<Jugadore> JugadoreCreadoPorNavigations { get; set; } = new List<Jugadore>();

    public virtual ICollection<JugadoresEquipo> JugadoresEquipoActualizadoPorNavigations { get; set; } = new List<JugadoresEquipo>();

    public virtual ICollection<JugadoresEquipo> JugadoresEquipoCreadoPorNavigations { get; set; } = new List<JugadoresEquipo>();

    public virtual ICollection<PagosCargo> PagosCargoActualizadoPorNavigations { get; set; } = new List<PagosCargo>();

    public virtual ICollection<PagosCargo> PagosCargoCreadoPorNavigations { get; set; } = new List<PagosCargo>();

    public virtual ICollection<Partido> PartidoActualizadoPorNavigations { get; set; } = new List<Partido>();

    public virtual ICollection<Partido> PartidoCreadoPorNavigations { get; set; } = new List<Partido>();

    public virtual ICollection<PartidosSancionCumplido> PartidosSancionCumplidoActualizadoPorNavigations { get; set; } = new List<PartidosSancionCumplido>();

    public virtual ICollection<PartidosSancionCumplido> PartidosSancionCumplidoCreadoPorNavigations { get; set; } = new List<PartidosSancionCumplido>();

    public virtual ICollection<Permiso> PermisoActualizadoPorNavigations { get; set; } = new List<Permiso>();

    public virtual ICollection<Permiso> PermisoCreadoPorNavigations { get; set; } = new List<Permiso>();

    public virtual ICollection<RevisionesVocalium> RevisionesVocaliumActualizadoPorNavigations { get; set; } = new List<RevisionesVocalium>();

    public virtual ICollection<RevisionesVocalium> RevisionesVocaliumCreadoPorNavigations { get; set; } = new List<RevisionesVocalium>();

    public virtual ICollection<Role> RoleActualizadoPorNavigations { get; set; } = new List<Role>();

    public virtual ICollection<Role> RoleCreadoPorNavigations { get; set; } = new List<Role>();

    public virtual ICollection<RolesPermiso> RolesPermisoActualizadoPorNavigations { get; set; } = new List<RolesPermiso>();

    public virtual ICollection<RolesPermiso> RolesPermisoCreadoPorNavigations { get; set; } = new List<RolesPermiso>();

    public virtual ICollection<SancionesJugador> SancionesJugadorActualizadoPorNavigations { get; set; } = new List<SancionesJugador>();

    public virtual ICollection<SancionesJugador> SancionesJugadorCreadoPorNavigations { get; set; } = new List<SancionesJugador>();

    public virtual ICollection<Temporada> TemporadaActualizadoPorNavigations { get; set; } = new List<Temporada>();

    public virtual ICollection<Temporada> TemporadaCreadoPorNavigations { get; set; } = new List<Temporada>();

    public virtual ICollection<UsuariosRole> UsuariosRoleActualizadoPorNavigations { get; set; } = new List<UsuariosRole>();

    public virtual ICollection<UsuariosRole> UsuariosRoleCreadoPorNavigations { get; set; } = new List<UsuariosRole>();

    public virtual ICollection<UsuariosRole> UsuariosRoleUsuarios { get; set; } = new List<UsuariosRole>();

    public virtual ICollection<Vocale> VocaleActualizadoPorNavigations { get; set; } = new List<Vocale>();

    public virtual ICollection<Vocale> VocaleCreadoPorNavigations { get; set; } = new List<Vocale>();

    public virtual Vocale? VocaleUsuario { get; set; }

    public virtual ICollection<Vocalias> VocaliasActualizadoPorNavigations { get; set; } = new List<Vocalias>();

    public virtual ICollection<Vocalias> VocaliasCreadoPorNavigations { get; set; } = new List<Vocalias>();
}
