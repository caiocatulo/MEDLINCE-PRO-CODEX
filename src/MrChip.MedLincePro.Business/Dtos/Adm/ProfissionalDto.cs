namespace MrChip.MedLincePro.Business.Dtos.Adm;

public sealed class ProfissionalDto
{
    public Guid ProfissionalId { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Registro { get; set; } = string.Empty;
    public int TipoDeDocumento { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public string ConselhoProfissionalCodigo { get; set; } = string.Empty;
    public string UnidadeFederacaoCodigo { get; set; } = string.Empty;
    public string CboCodigo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string EnderecoComplemento { get; set; } = string.Empty;
    public string EnderecoNumero { get; set; } = string.Empty;
    public bool PermiteAcessoAgenda { get; set; }
    public bool AcessoPlantao { get; set; }
    public bool ComissaoGlosas { get; set; }
    public string NumeroPis { get; set; } = string.Empty;
    public string CodigoGrauDeParticipacao { get; set; } = string.Empty;
    public string BancoCodigo { get; set; } = string.Empty;
    public string BancoAgencia { get; set; } = string.Empty;
    public string BancoConta { get; set; } = string.Empty;
    public EnderecoDto? Endereco { get; set; }
}
