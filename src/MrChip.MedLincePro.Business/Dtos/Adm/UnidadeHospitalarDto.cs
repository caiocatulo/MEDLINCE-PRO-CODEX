namespace MrChip.MedLincePro.Business.Dtos.Adm;

public sealed class UnidadeHospitalarDto
{
    public string CodigoCnes { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string UfCodigo { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string? Description { get; set; }
    public string NomeReduzido { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? ContatoEmail { get; set; }
    public string? ContatoNome { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
}
