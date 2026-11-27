namespace LigaArmenia.Domain.Entities;

public partial class Categoria
{
    public short Id { get; set; }

    public long TemporadaId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Disciplina { get; set; } = null!;

    public char? Serie { get; set; }

    public short? CupoEquipos { get; set; }

    public short PuntosVictoria { get; set; }

    public short PuntosEmpate { get; set; }

    public short PuntosDerrota { get; set; }

    public virtual Temporada Temporada { get; set; } = null!;

    public virtual ICollection<EtapaCategoria> Etapas { get; set; } = new List<EtapaCategoria>();

    public virtual ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();

    public virtual ICollection<Partido> Partidos { get; set; } = new List<Partido>();
}
