using MrChip.MedLincePro.Business.Models;

namespace MrChip.MedLincePro.Business.Models.Adm;

public sealed class Bureau : Entity
{
    public Bureau()
    {
        BureauId = Guid.NewGuid();
        Endereco = new Endereco();
    }

    public Guid BureauId { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeResumido { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public string Contato { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TelefoneFixo { get; set; } = string.Empty;
    public string TelefoneCel { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string? EnderecoComplemento { get; set; }
    public string EnderecoNumero { get; set; } = string.Empty;
    public DateTime ApiAcesso { get; set; }
    public string ApiIp { get; set; } = string.Empty;
    public int ApiAcessos { get; set; }
    public string ApiCulture { get; set; } = string.Empty;
    public string ApiCountry { get; set; } = string.Empty;
    public string Adm { get; set; } = string.Empty;
    public bool AcessoApi { get; set; }
    public int PrazoInclusaoGuia { get; set; }
    public Endereco Endereco { get; set; }
}
