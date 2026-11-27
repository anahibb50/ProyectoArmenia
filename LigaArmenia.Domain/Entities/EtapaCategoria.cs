namespace LigaArmenia.Domain.Entities;

public partial class EtapaCategoria
{
    public short Id { get; set; }

    public short CategoriaId { get; set; }

    public short Numero { get; set; }

    public string Nombre { get; set; } = null!;

    public string Formato { get; set; } = null!;

    public virtual Categoria Categoria { get; set; } = null!;
}
