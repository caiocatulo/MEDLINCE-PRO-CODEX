using MrChip.MedLincePro.Business.Models;

namespace MrChip.MedLincePro.Business.Models.Adm;

public sealed class Empresa : Entity
{
    public Empresa()
    {
        Endereco = new Endereco();
        Bureaux = new List<Bureau>();
    }

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
    public string EnderecoComplemento { get; set; } = string.Empty;
    public string EnderecoNumero { get; set; } = string.Empty;
    public string Cnes { get; set; } = string.Empty;
    public int EmpresaTipo { get; set; }
    public string LogoEmpresaMaior { get; set; } = string.Empty;
    public string LogoEmpresaMenor { get; set; } = string.Empty;
    public string LogoSistemaMenor { get; set; } = string.Empty;
    public string LogoSistemaMaior { get; set; } = string.Empty;
    public string LayoutPagina { get; set; } = string.Empty;
    public string UrlApi { get; set; } = string.Empty;
    public bool IntegraPlantao { get; set; }
    public string AppPlantao { get; set; } = string.Empty;
    public bool TrabalhaLoteGrupos { get; set; }
    public string PathArquivos { get; set; } = string.Empty;
    public Endereco Endereco { get; set; }
    public List<Bureau> Bureaux { get; set; }
}
