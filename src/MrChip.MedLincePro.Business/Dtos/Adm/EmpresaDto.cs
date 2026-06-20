namespace MrChip.MedLincePro.Business.Dtos.Adm;

public sealed class EmpresaDto
{
    public Guid EmpresaId { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TelefoneDdd { get; set; } = string.Empty;
    public string TelefoneNumero { get; set; } = string.Empty;
    public string ContatoNome { get; set; } = string.Empty;
    public string ContatoEmail { get; set; } = string.Empty;
    public string ContatoTelefoneDdd { get; set; } = string.Empty;
    public string ContatoTelefoneNumero { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string? EnderecoComplemento { get; set; }
    public string EnderecoNumero { get; set; } = string.Empty;
    public string? Cnes { get; set; }
    public int EmpresaTipo { get; set; }
    public string? LogoEmpresaMaior { get; set; }
    public string? LogoEmpresaMenor { get; set; }
    public string LogoSistemaMenor { get; set; } = string.Empty;
    public string LogoSistemaMaior { get; set; } = string.Empty;
    public string? LayoutPagina { get; set; }
    public string? UrlApi { get; set; }
    public bool? IntegraPlantao { get; set; }
    public string? AppPlantao { get; set; }
    public string? NumeroGuiaPrestador { get; set; }
    public bool? TrabalhaLoteGrupos { get; set; }
    public decimal? PercentualPIS { get; set; }
    public decimal? PercentualCOFINS { get; set; }
    public decimal? PercentualCSLL { get; set; }
    public decimal? PercentualIR { get; set; }
    public decimal? PercentualINSS { get; set; }
    public decimal? PercentualISS { get; set; }
    public decimal? TaxaAdm { get; set; }
    public string? PathArquivos { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public EnderecoDto? Endereco { get; set; }
    public List<BureauDto> Bureaux { get; set; } = new();
}
