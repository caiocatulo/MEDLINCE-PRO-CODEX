namespace MrChip.MedLincePro.Business.Models.Adm;

public sealed class Endereco
{
    public string Cep { get; set; } = string.Empty;
    public string? Logradouro { get; set; }
    public string? Bairro { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
