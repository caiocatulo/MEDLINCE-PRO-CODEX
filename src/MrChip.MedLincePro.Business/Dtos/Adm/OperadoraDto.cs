namespace MrChip.MedLincePro.Business.Dtos.Adm;

public sealed class OperadoraDto
{
    public Guid OperadoraId { get; set; }
    public Guid OperadoraModalidadeId { get; set; }
    public string RegistroAns { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public int CarteiraMin { get; set; }
    public int CarteiraMax { get; set; }
    public string? WsEndpoint { get; set; }
    public bool WsRetornaBeneficiario { get; set; }
    public string? CarteiraMask { get; set; }
    public int ElegibilidadeTipo { get; set; }
    public string? Token { get; set; }
    public string? ContatoEmail { get; set; }
    public string? ContatoNome { get; set; }
    public int SenhaNumCharSadt { get; set; }
    public int SenhaCharHonorario { get; set; }
    public int SenhaCharConsulta { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public EmpresaOperadoraDto? EmpresaOperadora { get; set; }
}

public sealed class EmpresaOperadoraDto
{
    public Guid EmpresaOperadoraId { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid OperadoraId { get; set; }
    public string? CodigoAuxiliar { get; set; }
    public string CodigoNaOperadora { get; set; } = string.Empty;
    public string? NumeroGuia { get; set; }
    public string NomeFantasia { get; set; } = string.Empty;
    public string? WsLogin { get; set; }
    public string? WsSenha { get; set; }
    public int WsSequencia { get; set; }
    public bool WsHabilitado { get; set; }
    public bool TrabalhaPlantao { get; set; }
    public bool FaturamentoTiss { get; set; }
    public bool NumeroGuiaObrigatorio { get; set; }
    public bool NumeroGuiaEmpresaObrigatorio { get; set; }
    public bool LiberadoParaAgendaMedica { get; set; }
    public bool AtendimentoUrgencia { get; set; }
    public bool AcrescimoCriancaIdoso { get; set; }
    public bool AtendimentoHorarioEspecial { get; set; }
    public bool AtendimentoApartamento { get; set; }
    public bool SenhaGuiaObrigatorio { get; set; }
    public bool ValidacaoGuiaHabilitado { get; set; }
    public bool LiberadoNovoFaturamento { get; set; }
    public string? VersaoXmlEnvio { get; set; }
    public string TipoIdentificacaoNaOperadora { get; set; } = string.Empty;
    public int FaturamentoLiberadoBureau { get; set; }
    public string? CompetenciaAtiva { get; set; }
    public int MaxGuiasLote { get; set; }
    public bool AutorizadoConsulta { get; set; }
    public bool AutorizadoTeleConsulta { get; set; }
    public Guid ConsultaGuiaTipoId { get; set; }
    public string? CobrancaSADTOrigem { get; set; }
    public string? CobrancaSADTSolicitante { get; set; }
    public string? CobrancaSADTExecutante { get; set; }
    public string? CobrancaHonorarioOrigem { get; set; }
    public string? CobrancaHonorarioContratado { get; set; }
    public string? CobrancaHonorarioExecutante { get; set; }
    public bool? NumeroGuiaPrincipalObrigatorio { get; set; }
    public bool? CopiarGuia { get; set; }
    public bool? CarteiraObrigatoria { get; set; }
    public bool? ContratoParticular { get; set; }
    public bool ConvenioPublico { get; set; }
    public string? WsMrChipUrl { get; set; }
    public bool? WsMrChipElegibilidade { get; set; }
    public int? CaraterDoAtendimentoPadrao { get; set; }
    public int? TipoAtendimentoPadrao { get; set; }
    public string? RegimeAtendimentoPadrao { get; set; }
    public bool? Checkin { get; set; }
    public bool? Checkout { get; set; }
    public bool? TrabalhaTecnica { get; set; }
    public string? TagNumeroGuiaPrestador { get; set; }
    public bool? HorarioObrigatorioXml { get; set; }
    public string? FormaCalculoProcedimento { get; set; }
    public int? ModeloGuiaAuxiliar { get; set; }
    public bool? ConcatenarGrupoProcedimento { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
}
