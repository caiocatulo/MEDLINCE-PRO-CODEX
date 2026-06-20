using MrChip.MedLincePro.Business.Models;

namespace MrChip.MedLincePro.Business.Models.Adm;

public sealed class Endereco : Entity
{
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
}
