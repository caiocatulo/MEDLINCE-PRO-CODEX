using MrChip.MedLincePro.Business.Models;

namespace MrChip.MedLincePro.Business.Models.Adm;

public sealed class Profissional : Entity
{
    public Profissional()
    {
        Endereco = new Endereco();
    }

    public Guid ProfissionalId { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Registro { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string ConselhoProfissionalCodigo { get; set; } = string.Empty;
    public string UnidadeFederacaoCodigo { get; set; } = string.Empty;
    public string CboCodigo { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string Cep { get; set; } = string.Empty;
    public string? EnderecoComplemento { get; set; }
    public string EnderecoNumero { get; set; } = string.Empty;
    public bool PermiteAcessoAgenda { get; set; }
    public bool AcessoPlantao { get; set; }
    public bool ComissaoGlosas { get; set; }
    public string NumeroPis { get; set; } = string.Empty;
    public string? CodigoGrauDeParticipacao { get; set; }
    public string? BancoCodigo { get; set; }
    public string? BancoAgencia { get; set; }
    public string? BancoConta { get; set; }
    public int? TipoDeDocumento { get; set; }
    public Endereco Endereco { get; set; }
}
