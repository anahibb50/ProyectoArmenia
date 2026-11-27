using System;
using System.Collections.Generic;
using LigaArmenia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LigaArmenia.Infrastructure.Data;

public partial class LigaArmeniaDbContext : DbContext
{
    public LigaArmeniaDbContext(DbContextOptions<LigaArmeniaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alineacione> Alineaciones { get; set; }

    public virtual DbSet<ArchivosVocalium> ArchivosVocalia { get; set; }

    public virtual DbSet<CambiosDesignacionVocal> CambiosDesignacionVocals { get; set; }

    public virtual DbSet<CambiosProgramacion> CambiosProgramacions { get; set; }

    public virtual DbSet<CargosPartido> CargosPartidos { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<DescansosEquipo> DescansosEquipos { get; set; }

    public virtual DbSet<DesignacionesVocalium> DesignacionesVocalia { get; set; }

    public virtual DbSet<Equipo> Equipos { get; set; }

    public virtual DbSet<EtapaCategoria> EtapasCategoria { get; set; }

    public virtual DbSet<EventosPartido> EventosPartidos { get; set; }

    public virtual DbSet<Horario> Horarios { get; set; }

    public virtual DbSet<Jugadore> Jugadores { get; set; }

    public virtual DbSet<JugadoresEquipo> JugadoresEquipos { get; set; }

    public virtual DbSet<PagosCargo> PagosCargos { get; set; }

    public virtual DbSet<Partido> Partidos { get; set; }

    public virtual DbSet<PartidosSancionCumplido> PartidosSancionCumplidos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Reunione> Reuniones { get; set; }

    public virtual DbSet<RevisionesVocalium> RevisionesVocalia { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesPermiso> RolesPermisos { get; set; }

    public virtual DbSet<SancionesJugador> SancionesJugadors { get; set; }

    public virtual DbSet<Temporada> Temporadas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuariosRole> UsuariosRoles { get; set; }

    public virtual DbSet<Vocale> Vocales { get; set; }

    public virtual DbSet<Vocalias> Vocaliases { get; set; }

    public virtual DbSet<VwCargosPendiente> VwCargosPendientes { get; set; }

    public virtual DbSet<VwGoleadore> VwGoleadores { get; set; }

    public virtual DbSet<VwSancionesPendiente> VwSancionesPendientes { get; set; }

    public virtual DbSet<VwTablaPosicione> VwTablaPosiciones { get; set; }

    public virtual DbSet<VwVocaliasPorRevisar> VwVocaliasPorRevisars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone", "recovery_code" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in", "like", "ilike", "is", "match", "imatch", "isdistinct" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Alineacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("alineaciones_pkey");

            entity.ToTable("alineaciones", "ldbla");

            entity.HasIndex(e => e.UuidSync, "alineaciones_uuid_sync_key").IsUnique();

            entity.HasIndex(e => new { e.VocaliaId, e.EquipoId }, "uq_alineacion_capitan")
                .IsUnique()
                .HasFilter("capitan");

            entity.HasIndex(e => new { e.VocaliaId, e.JugadorEquipoId }, "uq_alineacion_jugador").IsUnique();

            entity.HasIndex(e => new { e.VocaliaId, e.EquipoId, e.NumeroCamiseta }, "uq_alineacion_numero").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.Capitan)
                .HasDefaultValue(false)
                .HasColumnName("capitan");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.EquipoId).HasColumnName("equipo_id");
            entity.Property(e => e.JugadorEquipoId).HasColumnName("jugador_equipo_id");
            entity.Property(e => e.MinutoEntrada).HasColumnName("minuto_entrada");
            entity.Property(e => e.MinutoSalida).HasColumnName("minuto_salida");
            entity.Property(e => e.NumeroCamiseta).HasColumnName("numero_camiseta");
            entity.Property(e => e.Participo)
                .HasDefaultValue(false)
                .HasColumnName("participo");
            entity.Property(e => e.PresentoCarnet)
                .HasDefaultValue(true)
                .HasColumnName("presento_carnet");
            entity.Property(e => e.Titular)
                .HasDefaultValue(false)
                .HasColumnName("titular");
            entity.Property(e => e.UuidSync)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uuid_sync");
            entity.Property(e => e.VocaliaId).HasColumnName("vocalia_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.AlineacioneActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("alineaciones_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.AlineacioneCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("alineaciones_creado_por_fkey");

            entity.HasOne(d => d.Equipo).WithMany(p => p.Alineaciones)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_alineacion_equipo");

            entity.HasOne(d => d.Vocalia).WithMany(p => p.Alineaciones)
                .HasForeignKey(d => d.VocaliaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_alineacion_vocalia");

            entity.HasOne(d => d.JugadoresEquipo).WithMany(p => p.Alineaciones)
                .HasPrincipalKey(p => new { p.Id, p.EquipoId })
                .HasForeignKey(d => new { d.JugadorEquipoId, d.EquipoId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_alineacion_plantilla");
        });

        modelBuilder.Entity<ArchivosVocalium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("archivos_vocalia_pkey");

            entity.ToTable("archivos_vocalia", "ldbla");

            entity.HasIndex(e => e.UuidSync, "archivos_vocalia_uuid_sync_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.RegistradoEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("registrado_en");
            entity.Property(e => e.RutaArchivo)
                .HasMaxLength(500)
                .HasColumnName("ruta_archivo");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
            entity.Property(e => e.UuidSync)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uuid_sync");
            entity.Property(e => e.VocaliaId).HasColumnName("vocalia_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.ArchivosVocaliumActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("archivos_vocalia_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.ArchivosVocaliumCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("archivos_vocalia_creado_por_fkey");

            entity.HasOne(d => d.Vocalia).WithMany(p => p.ArchivosVocalia)
                .HasForeignKey(d => d.VocaliaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_archivo_vocalia");
        });

        modelBuilder.Entity<CambiosDesignacionVocal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cambios_designacion_vocal_pkey");

            entity.ToTable("cambios_designacion_vocal", "ldbla");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CambiadoPor).HasColumnName("cambiado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.EquipoAnteriorId).HasColumnName("equipo_anterior_id");
            entity.Property(e => e.EquipoNuevoId).HasColumnName("equipo_nuevo_id");
            entity.Property(e => e.Motivo).HasColumnName("motivo");
            entity.Property(e => e.PartidoId).HasColumnName("partido_id");
            entity.Property(e => e.RegistradoEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("registrado_en");
            entity.Property(e => e.VocalAnteriorId).HasColumnName("vocal_anterior_id");
            entity.Property(e => e.VocalNuevoId).HasColumnName("vocal_nuevo_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.CambiosDesignacionVocalActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("cambios_designacion_vocal_actualizado_por_fkey");

            entity.HasOne(d => d.CambiadoPorNavigation).WithMany(p => p.CambiosDesignacionVocalCambiadoPorNavigations)
                .HasForeignKey(d => d.CambiadoPor)
                .HasConstraintName("cambios_designacion_vocal_cambiado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.CambiosDesignacionVocalCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("cambios_designacion_vocal_creado_por_fkey");

            entity.HasOne(d => d.EquipoAnterior).WithMany(p => p.CambiosDesignacionVocalEquipoAnteriors)
                .HasForeignKey(d => d.EquipoAnteriorId)
                .HasConstraintName("fk_cdv_equipo_ant");

            entity.HasOne(d => d.EquipoNuevo).WithMany(p => p.CambiosDesignacionVocalEquipoNuevos)
                .HasForeignKey(d => d.EquipoNuevoId)
                .HasConstraintName("fk_cdv_equipo_nuevo");

            entity.HasOne(d => d.Partido).WithMany(p => p.CambiosDesignacionVocals)
                .HasForeignKey(d => d.PartidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cdv_partido");

            entity.HasOne(d => d.VocalAnterior).WithMany(p => p.CambiosDesignacionVocalVocalAnteriors)
                .HasForeignKey(d => d.VocalAnteriorId)
                .HasConstraintName("fk_cdv_vocal_ant");

            entity.HasOne(d => d.VocalNuevo).WithMany(p => p.CambiosDesignacionVocalVocalNuevos)
                .HasForeignKey(d => d.VocalNuevoId)
                .HasConstraintName("fk_cdv_vocal_nuevo");
        });

        modelBuilder.Entity<CambiosProgramacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cambios_programacion_pkey");

            entity.ToTable("cambios_programacion", "ldbla");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.FechaAnterior).HasColumnName("fecha_anterior");
            entity.Property(e => e.FechaNueva).HasColumnName("fecha_nueva");
            entity.Property(e => e.HorarioAnteriorId).HasColumnName("horario_anterior_id");
            entity.Property(e => e.HorarioNuevoId).HasColumnName("horario_nuevo_id");
            entity.Property(e => e.Motivo).HasColumnName("motivo");
            entity.Property(e => e.PartidoId).HasColumnName("partido_id");
            entity.Property(e => e.RealizadoPor).HasColumnName("realizado_por");
            entity.Property(e => e.RegistradoEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("registrado_en");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.CambiosProgramacionActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("cambios_programacion_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.CambiosProgramacionCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("cambios_programacion_creado_por_fkey");

            entity.HasOne(d => d.HorarioAnterior).WithMany(p => p.CambiosProgramacionHorarioAnteriors)
                .HasForeignKey(d => d.HorarioAnteriorId)
                .HasConstraintName("fk_cambios_hora_ant");

            entity.HasOne(d => d.HorarioNuevo).WithMany(p => p.CambiosProgramacionHorarioNuevos)
                .HasForeignKey(d => d.HorarioNuevoId)
                .HasConstraintName("fk_cambios_hora_nuevo");

            entity.HasOne(d => d.Partido).WithMany(p => p.CambiosProgramacions)
                .HasForeignKey(d => d.PartidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cambios_partido");

            entity.HasOne(d => d.RealizadoPorNavigation).WithMany(p => p.CambiosProgramacionRealizadoPorNavigations)
                .HasForeignKey(d => d.RealizadoPor)
                .HasConstraintName("cambios_programacion_realizado_por_fkey");
        });

        modelBuilder.Entity<CargosPartido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cargos_partido_pkey");

            entity.ToTable("cargos_partido", "ldbla");

            entity.HasIndex(e => new { e.EquipoId, e.Estado }, "ix_cargos_equipo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.Concepto)
                .HasMaxLength(200)
                .HasColumnName("concepto");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.EquipoId).HasColumnName("equipo_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasDefaultValueSql("'PROPUESTO'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.EventoId).HasColumnName("evento_id");
            entity.Property(e => e.Monto)
                .HasPrecision(10, 2)
                .HasColumnName("monto");
            entity.Property(e => e.PartidoId).HasColumnName("partido_id");
            entity.Property(e => e.ReunionId).HasColumnName("reunion_id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(15)
                .HasColumnName("tipo");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.CargosPartidoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("cargos_partido_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.CargosPartidoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("cargos_partido_creado_por_fkey");

            entity.HasOne(d => d.Equipo).WithMany(p => p.CargosPartidos)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cargo_equipo");

            entity.HasOne(d => d.Evento).WithMany(p => p.CargosPartidos)
                .HasForeignKey(d => d.EventoId)
                .HasConstraintName("fk_cargo_evento");

            entity.HasOne(d => d.Partido).WithMany(p => p.CargosPartidos)
                .HasForeignKey(d => d.PartidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cargo_partido");
        });

        modelBuilder.Entity<DescansosEquipo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("descansos_equipo_pkey");

            entity.ToTable("descansos_equipo", "ldbla");

            entity.HasIndex(e => new { e.EquipoId, e.FechaSabado }, "uq_descansos_equipo").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.EquipoId).HasColumnName("equipo_id");
            entity.Property(e => e.FechaSabado).HasColumnName("fecha_sabado");
            entity.Property(e => e.Motivo)
                .HasMaxLength(150)
                .HasDefaultValueSql("'ROTACION_2_JUEGA_1_DESCANSA'::character varying")
                .HasColumnName("motivo");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.DescansosEquipoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("descansos_equipo_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.DescansosEquipoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("descansos_equipo_creado_por_fkey");

            entity.HasOne(d => d.Equipo).WithMany(p => p.DescansosEquipos)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_descansos_equipo");
        });

        modelBuilder.Entity<DesignacionesVocalium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("designaciones_vocalia_pkey");

            entity.ToTable("designaciones_vocalia", "ldbla");

            entity.HasIndex(e => e.PartidoId, "designaciones_vocalia_partido_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.DesignadoEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("designado_en");
            entity.Property(e => e.EquipoResponsableId).HasColumnName("equipo_responsable_id");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.PartidoId).HasColumnName("partido_id");
            entity.Property(e => e.VocalId).HasColumnName("vocal_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.DesignacionesVocaliumActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("designaciones_vocalia_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.DesignacionesVocaliumCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("designaciones_vocalia_creado_por_fkey");

            entity.HasOne(d => d.EquipoResponsable).WithMany(p => p.DesignacionesVocalia)
                .HasForeignKey(d => d.EquipoResponsableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dv_equipo");

            entity.HasOne(d => d.Partido).WithOne(p => p.DesignacionesVocalium)
                .HasForeignKey<DesignacionesVocalium>(d => d.PartidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dv_partido");

            entity.HasOne(d => d.Vocal).WithMany(p => p.DesignacionesVocalia)
                .HasForeignKey(d => d.VocalId)
                .HasConstraintName("fk_dv_vocal");
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("equipos_pkey");

            entity.ToTable("equipos", "ldbla");

            entity.HasIndex(e => new { e.Id, e.TemporadaId, e.CategoriaId }, "uq_equipos_clave_compuesta").IsUnique();

            entity.HasIndex(e => new { e.TemporadaId, e.CategoriaId, e.Nombre }, "uq_equipos_nombre_categoria").IsUnique();

            entity.HasIndex(e => e.CategoriaId, "ix_equipos_categoria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Nombre)
                .HasMaxLength(140)
                .HasColumnName("nombre");
            entity.Property(e => e.Serie)
                .HasMaxLength(1)
                .HasColumnName("serie");
            entity.Property(e => e.TemporadaId).HasColumnName("temporada_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.EquipoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("equipos_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.EquipoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("equipos_creado_por_fkey");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_equipos_categoria");

            entity.HasOne(d => d.Temporada).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.TemporadaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_equipos_temporada");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categorias_pkey");

            entity.ToTable("categorias", "ldbla");

            entity.HasIndex(e => new { e.TemporadaId, e.Codigo }, "uq_categorias_codigo").IsUnique();

            entity.HasIndex(e => new { e.Id, e.TemporadaId }, "uq_categorias_id_temporada").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.CupoEquipos).HasColumnName("cupo_equipos");
            entity.Property(e => e.Disciplina)
                .HasMaxLength(20)
                .HasColumnName("disciplina");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .HasColumnName("nombre");
            entity.Property(e => e.PuntosDerrota)
                .HasDefaultValue((short)0)
                .HasColumnName("puntos_derrota");
            entity.Property(e => e.PuntosEmpate)
                .HasDefaultValue((short)1)
                .HasColumnName("puntos_empate");
            entity.Property(e => e.PuntosVictoria)
                .HasDefaultValue((short)3)
                .HasColumnName("puntos_victoria");
            entity.Property(e => e.Serie)
                .HasMaxLength(1)
                .HasColumnName("serie");
            entity.Property(e => e.TemporadaId).HasColumnName("temporada_id");

            entity.HasOne(d => d.Temporada).WithMany(p => p.Categorias)
                .HasForeignKey(d => d.TemporadaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_categorias_temporada");
        });

        modelBuilder.Entity<EtapaCategoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("etapas_categoria_pkey");

            entity.ToTable("etapas_categoria", "ldbla");

            entity.HasIndex(e => new { e.CategoriaId, e.Numero }, "uq_etapas_categoria").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.Formato)
                .HasMaxLength(30)
                .HasColumnName("formato");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .HasColumnName("nombre");
            entity.Property(e => e.Numero).HasColumnName("numero");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Etapas)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("fk_etapas_categoria");
        });

        modelBuilder.Entity<EventosPartido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventos_partido_pkey");

            entity.ToTable("eventos_partido", "ldbla");

            entity.HasIndex(e => e.UuidSync, "eventos_partido_uuid_sync_key").IsUnique();

            entity.HasIndex(e => new { e.VocaliaId, e.Tipo, e.Valido }, "ix_evento_busqueda");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.EquipoId).HasColumnName("equipo_id");
            entity.Property(e => e.JugadorEquipoId).HasColumnName("jugador_equipo_id");
            entity.Property(e => e.JugadorRelacionadoId).HasColumnName("jugador_relacionado_id");
            entity.Property(e => e.Minuto).HasColumnName("minuto");
            entity.Property(e => e.Motivo)
                .HasMaxLength(250)
                .HasColumnName("motivo");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
            entity.Property(e => e.UuidSync)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uuid_sync");
            entity.Property(e => e.Valido)
                .HasDefaultValue(true)
                .HasColumnName("valido");
            entity.Property(e => e.VocaliaId).HasColumnName("vocalia_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.EventosPartidoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("eventos_partido_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.EventosPartidoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("eventos_partido_creado_por_fkey");

            entity.HasOne(d => d.Equipo).WithMany(p => p.EventosPartidos)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_evento_equipo");

            entity.HasOne(d => d.Vocalia).WithMany(p => p.EventosPartidos)
                .HasForeignKey(d => d.VocaliaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_evento_vocalia");

            entity.HasOne(d => d.JugadoresEquipo).WithMany(p => p.EventosPartidoJugadoresEquipos)
                .HasPrincipalKey(p => new { p.Id, p.EquipoId })
                .HasForeignKey(d => new { d.JugadorEquipoId, d.EquipoId })
                .HasConstraintName("fk_evento_jugador");

            entity.HasOne(d => d.JugadoresEquipoNavigation).WithMany(p => p.EventosPartidoJugadoresEquipoNavigations)
                .HasPrincipalKey(p => new { p.Id, p.EquipoId })
                .HasForeignKey(d => new { d.JugadorRelacionadoId, d.EquipoId })
                .HasConstraintName("fk_evento_relacionado");
        });

        modelBuilder.Entity<Horario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("horarios_pkey");

            entity.ToTable("horarios", "ldbla");

            entity.HasIndex(e => new { e.HoraInicio, e.HoraFin }, "uq_horarios").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.HorarioActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("horarios_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.HorarioCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("horarios_creado_por_fkey");
        });

        modelBuilder.Entity<Jugadore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jugadores_pkey");

            entity.ToTable("jugadores", "ldbla");

            entity.HasIndex(e => e.Documento, "jugadores_documento_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Documento)
                .HasMaxLength(35)
                .HasColumnName("documento");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Nombres)
                .HasMaxLength(180)
                .HasColumnName("nombres");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.JugadoreActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("jugadores_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.JugadoreCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("jugadores_creado_por_fkey");
        });

        modelBuilder.Entity<JugadoresEquipo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jugadores_equipo_pkey");

            entity.ToTable("jugadores_equipo", "ldbla");

            entity.HasIndex(e => new { e.EquipoId, e.Habilitado }, "ix_je_equipo_habilitado");

            entity.HasIndex(e => new { e.JugadorId, e.EquipoId, e.FechaAlta }, "uq_je_alta").IsUnique();

            entity.HasIndex(e => new { e.Id, e.EquipoId }, "uq_je_clave_compuesta").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.ConsideradoJuvenil)
                .HasDefaultValue(false)
                .HasColumnName("considerado_juvenil");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.EquipoId).HasColumnName("equipo_id");
            entity.Property(e => e.FechaAlta).HasColumnName("fecha_alta");
            entity.Property(e => e.FechaBaja).HasColumnName("fecha_baja");
            entity.Property(e => e.Habilitado)
                .HasDefaultValue(true)
                .HasColumnName("habilitado");
            entity.Property(e => e.JugadorId).HasColumnName("jugador_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.JugadoresEquipoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("jugadores_equipo_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.JugadoresEquipoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("jugadores_equipo_creado_por_fkey");

            entity.HasOne(d => d.Equipo).WithMany(p => p.JugadoresEquipos)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_je_equipo");

            entity.HasOne(d => d.Jugador).WithMany(p => p.JugadoresEquipos)
                .HasForeignKey(d => d.JugadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_je_jugador");
        });

        modelBuilder.Entity<PagosCargo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pagos_cargo_pkey");

            entity.ToTable("pagos_cargo", "ldbla");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CargoId).HasColumnName("cargo_id");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Monto)
                .HasPrecision(10, 2)
                .HasColumnName("monto");
            entity.Property(e => e.PagadoEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("pagado_en");
            entity.Property(e => e.Referencia)
                .HasMaxLength(120)
                .HasColumnName("referencia");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.PagosCargoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("pagos_cargo_actualizado_por_fkey");

            entity.HasOne(d => d.Cargo).WithMany(p => p.PagosCargos)
                .HasForeignKey(d => d.CargoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pago_cargo");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.PagosCargoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("pagos_cargo_creado_por_fkey");
        });

        modelBuilder.Entity<Partido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("partidos_pkey");

            entity.ToTable("partidos", "ldbla");

            entity.HasIndex(e => new { e.Fecha, e.Estado }, "ix_partidos_calendario");

            entity.HasIndex(e => new { e.EquipoLocalId, e.Fecha }, "ix_partidos_local");

            entity.HasIndex(e => new { e.EquipoVisitanteId, e.Fecha }, "ix_partidos_visitante");

            entity.HasIndex(e => new { e.Fecha, e.HorarioId }, "uq_partidos_unica_cancha").IsUnique();

            entity.HasIndex(e => e.CategoriaId, "ix_partidos_categoria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.AprobadoEn).HasColumnName("aprobado_en");
            entity.Property(e => e.CostoArbitrajeTotal)
                .HasPrecision(10, 2)
                .HasColumnName("costo_arbitraje_total");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.EquipoLocalId).HasColumnName("equipo_local_id");
            entity.Property(e => e.EquipoVisitanteId).HasColumnName("equipo_visitante_id");
            entity.Property(e => e.Estado)
                .HasMaxLength(25)
                .HasDefaultValueSql("'BORRADOR'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.Etapa)
                .HasDefaultValue((short)1)
                .HasColumnName("etapa");
            entity.Property(e => e.Fase)
                .HasMaxLength(20)
                .HasDefaultValueSql("'REGULAR'::character varying")
                .HasColumnName("fase");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.GolesLocalOficial).HasColumnName("goles_local_oficial");
            entity.Property(e => e.GolesVisitanteOficial).HasColumnName("goles_visitante_oficial");
            entity.Property(e => e.GrupoFase)
                .HasMaxLength(10)
                .HasColumnName("grupo_fase");
            entity.Property(e => e.HorarioId).HasColumnName("horario_id");
            entity.Property(e => e.MotivoResultado).HasColumnName("motivo_resultado");
            entity.Property(e => e.Serie)
                .HasMaxLength(1)
                .HasColumnName("serie");
            entity.Property(e => e.TemporadaId).HasColumnName("temporada_id");
            entity.Property(e => e.TipoResultado)
                .HasMaxLength(20)
                .HasColumnName("tipo_resultado");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.PartidoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("partidos_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.PartidoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("partidos_creado_por_fkey");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Partidos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_partidos_categoria");

            entity.HasOne(d => d.Horario).WithMany(p => p.Partidos)
                .HasForeignKey(d => d.HorarioId)
                .HasConstraintName("fk_partidos_horario");

            entity.HasOne(d => d.Temporada).WithMany(p => p.Partidos)
                .HasForeignKey(d => d.TemporadaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_partidos_temporada");

            entity.HasOne(d => d.Equipo).WithMany(p => p.PartidoEquipos)
                .HasPrincipalKey(p => new { p.Id, p.TemporadaId, p.CategoriaId })
                .HasForeignKey(d => new { d.EquipoLocalId, d.TemporadaId, d.CategoriaId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_partidos_local");

            entity.HasOne(d => d.EquipoNavigation).WithMany(p => p.PartidoEquipoNavigations)
                .HasPrincipalKey(p => new { p.Id, p.TemporadaId, p.CategoriaId })
                .HasForeignKey(d => new { d.EquipoVisitanteId, d.TemporadaId, d.CategoriaId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_partidos_visitante");
        });

        modelBuilder.Entity<PartidosSancionCumplido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("partidos_sancion_cumplidos_pkey");

            entity.ToTable("partidos_sancion_cumplidos", "ldbla");

            entity.HasIndex(e => new { e.SancionId, e.PartidoId }, "uq_cumplimiento").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.PartidoId).HasColumnName("partido_id");
            entity.Property(e => e.RegistradoEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("registrado_en");
            entity.Property(e => e.SancionId).HasColumnName("sancion_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.PartidosSancionCumplidoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("partidos_sancion_cumplidos_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.PartidosSancionCumplidoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("partidos_sancion_cumplidos_creado_por_fkey");

            entity.HasOne(d => d.Partido).WithMany(p => p.PartidosSancionCumplidos)
                .HasForeignKey(d => d.PartidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cumplimiento_partido");

            entity.HasOne(d => d.Sancion).WithMany(p => p.PartidosSancionCumplidos)
                .HasForeignKey(d => d.SancionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cumplimiento_sancion");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permisos_pkey");

            entity.ToTable("permisos", "ldbla");

            entity.HasIndex(e => e.Codigo, "permisos_codigo_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.Codigo)
                .HasMaxLength(100)
                .HasColumnName("codigo");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.PermisoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("permisos_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.PermisoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("permisos_creado_por_fkey");
        });

        modelBuilder.Entity<Reunione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reuniones_pkey");

            entity.ToTable("reuniones", "ldbla");

            entity.HasIndex(e => e.NumeroActa, "reuniones_numero_acta_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.NumeroActa)
                .HasMaxLength(50)
                .HasColumnName("numero_acta");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasDefaultValueSql("'TECNICA'::character varying")
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<RevisionesVocalium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("revisiones_vocalia_pkey");

            entity.ToTable("revisiones_vocalia", "ldbla");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Decision)
                .HasMaxLength(20)
                .HasColumnName("decision");
            entity.Property(e => e.Fundamento).HasColumnName("fundamento");
            entity.Property(e => e.GolesLocalResueltos).HasColumnName("goles_local_resueltos");
            entity.Property(e => e.GolesVisitanteResueltos).HasColumnName("goles_visitante_resueltos");
            entity.Property(e => e.ReunionId).HasColumnName("reunion_id");
            entity.Property(e => e.RevisadaEn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("revisada_en");
            entity.Property(e => e.TipoResultado)
                .HasMaxLength(20)
                .HasColumnName("tipo_resultado");
            entity.Property(e => e.VocaliaId).HasColumnName("vocalia_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.RevisionesVocaliumActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("revisiones_vocalia_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.RevisionesVocaliumCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("revisiones_vocalia_creado_por_fkey");

            entity.HasOne(d => d.Vocalia).WithMany(p => p.RevisionesVocalia)
                .HasForeignKey(d => d.VocaliaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_revision_vocalia");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles", "ldbla");

            entity.HasIndex(e => e.Nombre, "roles_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .HasColumnName("nombre");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.RoleActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("roles_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.RoleCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("roles_creado_por_fkey");
        });

        modelBuilder.Entity<RolesPermiso>(entity =>
        {
            entity.HasKey(e => new { e.RolId, e.PermisoId }).HasName("roles_permisos_pkey");

            entity.ToTable("roles_permisos", "ldbla");

            entity.HasIndex(e => e.PermisoId, "ix_roles_permisos_permiso");

            entity.Property(e => e.RolId).HasColumnName("rol_id");
            entity.Property(e => e.PermisoId).HasColumnName("permiso_id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.RolesPermisoActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("roles_permisos_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.RolesPermisoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("roles_permisos_creado_por_fkey");

            entity.HasOne(d => d.Permiso).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.PermisoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_permisos_permiso_id_fkey");

            entity.HasOne(d => d.Rol).WithMany(p => p.RolesPermisos)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_permisos_rol_id_fkey");
        });

        modelBuilder.Entity<SancionesJugador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sanciones_jugador_pkey");

            entity.ToTable("sanciones_jugador", "ldbla");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.Articulo)
                .HasMaxLength(40)
                .HasColumnName("articulo");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasDefaultValueSql("'PREVENTIVA'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.JugadorId).HasColumnName("jugador_id");
            entity.Property(e => e.Motivo)
                .HasMaxLength(350)
                .HasColumnName("motivo");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.PartidosImpuestos).HasColumnName("partidos_impuestos");
            entity.Property(e => e.ResueltaEn).HasColumnName("resuelta_en");
            entity.Property(e => e.ReunionId).HasColumnName("reunion_id");
            entity.Property(e => e.VocaliaOrigenId).HasColumnName("vocalia_origen_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.SancionesJugadorActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("sanciones_jugador_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.SancionesJugadorCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("sanciones_jugador_creado_por_fkey");

            entity.HasOne(d => d.Jugador).WithMany(p => p.SancionesJugadors)
                .HasForeignKey(d => d.JugadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sancion_jugador");

            entity.HasOne(d => d.VocaliaOrigen).WithMany(p => p.SancionesJugadors)
                .HasForeignKey(d => d.VocaliaOrigenId)
                .HasConstraintName("fk_sancion_vocalia");
        });

        modelBuilder.Entity<Temporada>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("temporadas_pkey");

            entity.ToTable("temporadas", "ldbla");

            entity.HasIndex(e => e.Nombre, "temporadas_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activa)
                .HasDefaultValue(true)
                .HasColumnName("activa");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.TemporadaActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("temporadas_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.TemporadaCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("temporadas_creado_por_fkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuarios_pkey");

            entity.ToTable("usuarios", "ldbla");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.Correo)
                .HasMaxLength(254)
                .HasColumnName("correo");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Nombres)
                .HasMaxLength(180)
                .HasColumnName("nombres");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.InverseActualizadoPorNavigation)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("usuarios_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.InverseCreadoPorNavigation)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("usuarios_creado_por_fkey");
        });

        modelBuilder.Entity<UsuariosRole>(entity =>
        {
            entity.HasKey(e => new { e.UsuarioId, e.RolId }).HasName("usuarios_roles_pkey");

            entity.ToTable("usuarios_roles", "ldbla");

            entity.HasIndex(e => e.RolId, "ix_usuarios_roles_rol");

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.RolId).HasColumnName("rol_id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.UsuariosRoleActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("usuarios_roles_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.UsuariosRoleCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("usuarios_roles_creado_por_fkey");

            entity.HasOne(d => d.Rol).WithMany(p => p.UsuariosRoles)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuarios_roles_rol_id_fkey");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuariosRoleUsuarios)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuarios_roles_usuario_id_fkey");
        });

        modelBuilder.Entity<Vocale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vocales_pkey");

            entity.ToTable("vocales", "ldbla");

            entity.HasIndex(e => e.UsuarioId, "vocales_usuario_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Nombres)
                .HasMaxLength(180)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(35)
                .HasColumnName("telefono");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.VocaleActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("vocales_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.VocaleCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("vocales_creado_por_fkey");

            entity.HasOne(d => d.Usuario).WithOne(p => p.VocaleUsuario)
                .HasForeignKey<Vocale>(d => d.UsuarioId)
                .HasConstraintName("vocales_usuario_id_fkey");
        });

        modelBuilder.Entity<Vocalias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vocalias_pkey");

            entity.ToTable("vocalias", "ldbla");

            entity.HasIndex(e => e.PartidoId, "vocalias_partido_id_key").IsUnique();

            entity.HasIndex(e => e.UuidSync, "vocalias_uuid_sync_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualizadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("actualizado_en");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
            entity.Property(e => e.AprobadaEn).HasColumnName("aprobada_en");
            entity.Property(e => e.ArbitroLinea1)
                .HasMaxLength(170)
                .HasColumnName("arbitro_linea_1");
            entity.Property(e => e.ArbitroLinea2)
                .HasMaxLength(170)
                .HasColumnName("arbitro_linea_2");
            entity.Property(e => e.ArbitroPrincipal)
                .HasMaxLength(170)
                .HasColumnName("arbitro_principal");
            entity.Property(e => e.CerradaEn).HasColumnName("cerrada_en");
            entity.Property(e => e.CreadoEn)
                .HasDefaultValueSql("now()")
                .HasColumnName("creado_en");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.EnviadaRevisionEn).HasColumnName("enviada_revision_en");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'BORRADOR'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FinReal).HasColumnName("fin_real");
            entity.Property(e => e.FirmaArbitro)
                .HasDefaultValue(false)
                .HasColumnName("firma_arbitro");
            entity.Property(e => e.FirmaCapitanLocal)
                .HasDefaultValue(false)
                .HasColumnName("firma_capitan_local");
            entity.Property(e => e.FirmaCapitanVisitante)
                .HasDefaultValue(false)
                .HasColumnName("firma_capitan_visitante");
            entity.Property(e => e.FirmaVocal)
                .HasDefaultValue(false)
                .HasColumnName("firma_vocal");
            entity.Property(e => e.GolesLocalCancha).HasColumnName("goles_local_cancha");
            entity.Property(e => e.GolesVisitanteCancha).HasColumnName("goles_visitante_cancha");
            entity.Property(e => e.IncidentesGenerales).HasColumnName("incidentes_generales");
            entity.Property(e => e.InformeArbitro).HasColumnName("informe_arbitro");
            entity.Property(e => e.InformeVocal).HasColumnName("informe_vocal");
            entity.Property(e => e.InicioReal).HasColumnName("inicio_real");
            entity.Property(e => e.MalaVocalia)
                .HasDefaultValue(false)
                .HasColumnName("mala_vocalia");
            entity.Property(e => e.NoHizoVocalia)
                .HasDefaultValue(false)
                .HasColumnName("no_hizo_vocalia");
            entity.Property(e => e.NoPresentoLocal)
                .HasDefaultValue(false)
                .HasColumnName("no_presento_local");
            entity.Property(e => e.NoPresentoVisitante)
                .HasDefaultValue(false)
                .HasColumnName("no_presento_visitante");
            entity.Property(e => e.ObservacionesCapitanLocal).HasColumnName("observaciones_capitan_local");
            entity.Property(e => e.ObservacionesCapitanVisitante).HasColumnName("observaciones_capitan_visitante");
            entity.Property(e => e.PagosDeudasNotas).HasColumnName("pagos_deudas_notas");
            entity.Property(e => e.PartidoId).HasColumnName("partido_id");
            entity.Property(e => e.UuidSync)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("uuid_sync");
            entity.Property(e => e.Version)
                .HasDefaultValue(1)
                .HasColumnName("version");

            entity.HasOne(d => d.ActualizadoPorNavigation).WithMany(p => p.VocaliasActualizadoPorNavigations)
                .HasForeignKey(d => d.ActualizadoPor)
                .HasConstraintName("vocalias_actualizado_por_fkey");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.VocaliasCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("vocalias_creado_por_fkey");

            entity.HasOne(d => d.Partido).WithOne(p => p.Vocalias)
                .HasForeignKey<Vocalias>(d => d.PartidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vocalias_partido");
        });

        modelBuilder.Entity<VwCargosPendiente>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_cargos_pendientes", "ldbla");

            entity.Property(e => e.CargoId).HasColumnName("cargo_id");
            entity.Property(e => e.Concepto)
                .HasMaxLength(200)
                .HasColumnName("concepto");
            entity.Property(e => e.Equipo)
                .HasMaxLength(140)
                .HasColumnName("equipo");
            entity.Property(e => e.EquipoId).HasColumnName("equipo_id");
            entity.Property(e => e.Monto)
                .HasPrecision(10, 2)
                .HasColumnName("monto");
            entity.Property(e => e.Pagado).HasColumnName("pagado");
            entity.Property(e => e.PartidoId).HasColumnName("partido_id");
            entity.Property(e => e.Saldo).HasColumnName("saldo");
            entity.Property(e => e.Tipo)
                .HasMaxLength(15)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<VwGoleadore>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_goleadores", "ldbla");

            entity.Property(e => e.Equipo)
                .HasMaxLength(140)
                .HasColumnName("equipo");
            entity.Property(e => e.EquipoId).HasColumnName("equipo_id");
            entity.Property(e => e.Goles).HasColumnName("goles");
            entity.Property(e => e.Jugador)
                .HasMaxLength(180)
                .HasColumnName("jugador");
            entity.Property(e => e.JugadorId).HasColumnName("jugador_id");
            entity.Property(e => e.Serie)
                .HasMaxLength(1)
                .HasColumnName("serie");
            entity.Property(e => e.TemporadaId).HasColumnName("temporada_id");
        });

        modelBuilder.Entity<VwSancionesPendiente>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_sanciones_pendientes", "ldbla");

            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasColumnName("estado");
            entity.Property(e => e.Jugador)
                .HasMaxLength(180)
                .HasColumnName("jugador");
            entity.Property(e => e.JugadorId).HasColumnName("jugador_id");
            entity.Property(e => e.Motivo)
                .HasMaxLength(350)
                .HasColumnName("motivo");
            entity.Property(e => e.PartidosCumplidos).HasColumnName("partidos_cumplidos");
            entity.Property(e => e.PartidosImpuestos).HasColumnName("partidos_impuestos");
            entity.Property(e => e.PartidosPendientes).HasColumnName("partidos_pendientes");
            entity.Property(e => e.SancionId).HasColumnName("sancion_id");
        });

        modelBuilder.Entity<VwTablaPosicione>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_tabla_posiciones", "ldbla");

            entity.Property(e => e.Dg).HasColumnName("dg");
            entity.Property(e => e.Equipo)
                .HasMaxLength(140)
                .HasColumnName("equipo");
            entity.Property(e => e.EquipoId).HasColumnName("equipo_id");
            entity.Property(e => e.Etapa).HasColumnName("etapa");
            entity.Property(e => e.Fase)
                .HasMaxLength(20)
                .HasColumnName("fase");
            entity.Property(e => e.Gc).HasColumnName("gc");
            entity.Property(e => e.Gf).HasColumnName("gf");
            entity.Property(e => e.GrupoFase)
                .HasMaxLength(10)
                .HasColumnName("grupo_fase");
            entity.Property(e => e.Pe).HasColumnName("pe");
            entity.Property(e => e.Pg).HasColumnName("pg");
            entity.Property(e => e.Pj).HasColumnName("pj");
            entity.Property(e => e.Pp).HasColumnName("pp");
            entity.Property(e => e.Pts).HasColumnName("pts");
            entity.Property(e => e.Serie)
                .HasMaxLength(1)
                .HasColumnName("serie");
            entity.Property(e => e.TemporadaId).HasColumnName("temporada_id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(20)
                .HasColumnName("categoria");
        });

        modelBuilder.Entity<VwVocaliasPorRevisar>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_vocalias_por_revisar", "ldbla");

            entity.Property(e => e.EquipoLocal)
                .HasMaxLength(140)
                .HasColumnName("equipo_local");
            entity.Property(e => e.EquipoResponsableId).HasColumnName("equipo_responsable_id");
            entity.Property(e => e.EquipoVisitante)
                .HasMaxLength(140)
                .HasColumnName("equipo_visitante");
            entity.Property(e => e.EstadoVocalia)
                .HasMaxLength(20)
                .HasColumnName("estado_vocalia");
            entity.Property(e => e.Etapa).HasColumnName("etapa");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.PartidoId).HasColumnName("partido_id");
            entity.Property(e => e.Serie)
                .HasMaxLength(1)
                .HasColumnName("serie");
            entity.Property(e => e.VocalDesignado)
                .HasMaxLength(180)
                .HasColumnName("vocal_designado");
            entity.Property(e => e.VocalId).HasColumnName("vocal_id");
            entity.Property(e => e.VocaliaId).HasColumnName("vocalia_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
