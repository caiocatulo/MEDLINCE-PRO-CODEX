namespace MrChip.MedLincePro.Business.Models;

public abstract class Entity
{
    protected Entity()
    {
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
}
