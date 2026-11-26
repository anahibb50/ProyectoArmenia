namespace LigaArmenia.Domain.Constants;

/// <summary>
/// Códigos oficiales de permisos registrados en la tabla ldbla.permisos.
/// Usar estas constantes con [Authorize(Policy = SystemPermissions.UsuariosConsultar)].
/// </summary>
public static class SystemPermissions
{
    // Usuarios
    public const string UsuariosConsultar = "usuarios.consultar";
    public const string UsuariosCrear = "usuarios.crear";
    public const string UsuariosEditar = "usuarios.editar";
    public const string UsuariosDesactivar = "usuarios.desactivar";

    // Roles y permisos
    public const string RolesConsultar = "roles.consultar";
    public const string RolesGestionar = "roles.gestionar";
    public const string PermisosConsultar = "permisos.consultar";
    public const string PermisosAsignar = "permisos.asignar";

    // Auditoría
    public const string AuditoriaConsultar = "auditoria.consultar";

    // Campeonatos
    public const string CampeonatosConsultar = "campeonatos.consultar";
    public const string CampeonatosGestionar = "campeonatos.gestionar";

    // Equipos
    public const string EquiposConsultar = "equipos.consultar";
    public const string EquiposGestionar = "equipos.gestionar";

    // Jugadores
    public const string JugadoresConsultar = "jugadores.consultar";
    public const string JugadoresGestionar = "jugadores.gestionar";

    // Horarios
    public const string HorariosConsultar = "horarios.consultar";
    public const string HorariosGestionar = "horarios.gestionar";

    // Partidos
    public const string PartidosConsultar = "partidos.consultar";
    public const string PartidosGestionar = "partidos.gestionar";

    // Alineaciones
    public const string AlineacionesConsultar = "alineaciones.consultar";
    public const string AlineacionesGestionar = "alineaciones.gestionar";

    // Vocales (gestión administrativa de perfiles de vocal)
    public const string VocalesConsultar = "vocales.consultar";
    public const string VocalesEditar = "vocales.editar";
    public const string VocalesActivar = "vocales.activar";
    public const string VocalesDesactivar = "vocales.desactivar";

    // Vocalías
    public const string VocaliasConsultar = "vocalias.consultar";
    public const string VocaliasRegistrar = "vocalias.registrar";
    public const string VocaliasEditar = "vocalias.editar";
    public const string VocaliasEnviar = "vocalias.enviar";
    public const string VocaliasRevisar = "vocalias.revisar";

    // Tarjetas y sanciones
    public const string TarjetasConsultar = "tarjetas.consultar";
    public const string TarjetasRegistrar = "tarjetas.registrar";
    public const string SancionesConsultar = "sanciones.consultar";
    public const string SancionesGestionar = "sanciones.gestionar";

    // Reportes y posiciones
    public const string PosicionesConsultar = "posiciones.consultar";
    public const string ReportesConsultar = "reportes.consultar";

    /// <summary>
    /// Permisos críticos de seguridad que el rol ADMINISTRADOR nunca debe perder.
    /// </summary>
    public static readonly IReadOnlySet<string> CriticalAdminPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        UsuariosConsultar,
        UsuariosCrear,
        UsuariosEditar,
        UsuariosDesactivar,
        RolesConsultar,
        RolesGestionar,
        PermisosConsultar,
        PermisosAsignar,
        VocalesConsultar,
        VocalesEditar,
        VocalesActivar,
        VocalesDesactivar
    };
}
