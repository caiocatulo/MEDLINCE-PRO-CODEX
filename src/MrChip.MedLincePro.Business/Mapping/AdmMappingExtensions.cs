using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Models.Adm;

namespace MrChip.MedLincePro.Business.Mapping;

public static class AdmMappingExtensions
{
    public static EnderecoDto ToDto(this Endereco entity) => new()
    {
        Cep = entity.Cep,
        Logradouro = entity.Logradouro,
        Bairro = entity.Bairro,
        Cidade = entity.Cidade,
        Uf = entity.Uf,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo
    };

    public static BureauDto ToDto(this Bureau entity) => new()
    {
        BureauId = entity.BureauId,
        EmpresaId = entity.EmpresaId,
        Nome = entity.Nome,
        NomeResumido = entity.NomeResumido,
        CpfCnpj = entity.CpfCnpj,
        Contato = entity.Contato,
        Email = entity.Email,
        TelefoneFixo = entity.TelefoneFixo,
        TelefoneCel = entity.TelefoneCel,
        Cep = entity.Cep,
        EnderecoComplemento = entity.EnderecoComplemento,
        EnderecoNumero = entity.EnderecoNumero,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo,
        ApiAcesso = entity.ApiAcesso,
        ApiIp = entity.ApiIp,
        ApiAcessos = entity.ApiAcessos,
        ApiCulture = entity.ApiCulture,
        ApiCountry = entity.ApiCountry,
        AcessoApi = entity.AcessoApi,
        Adm = entity.Adm,
        PrazoInclusaoGuia = entity.PrazoInclusaoGuia,
        Endereco = entity.Endereco?.ToDto()
    };

    public static EmpresaDto ToDto(this Empresa entity) => new()
    {
        EmpresaId = entity.EmpresaId,
        RazaoSocial = entity.RazaoSocial,
        NomeFantasia = entity.NomeFantasia,
        Cnpj = entity.Cnpj,
        Email = entity.Email,
        TelefoneDdd = entity.TelefoneDdd,
        TelefoneNumero = entity.TelefoneNumero,
        ContatoNome = entity.ContatoNome,
        ContatoEmail = entity.ContatoEmail,
        ContatoTelefoneDdd = entity.ContatoTelefoneDdd,
        ContatoTelefoneNumero = entity.ContatoTelefoneNumero,
        Cep = entity.Cep,
        EnderecoComplemento = entity.EnderecoComplemento,
        EnderecoNumero = entity.EnderecoNumero,
        Cnes = entity.Cnes,
        EmpresaTipo = entity.EmpresaTipo,
        LogoEmpresaMaior = entity.LogoEmpresaMaior,
        LogoEmpresaMenor = entity.LogoEmpresaMenor,
        LogoSistemaMenor = entity.LogoSistemaMenor,
        LogoSistemaMaior = entity.LogoSistemaMaior,
        LayoutPagina = entity.LayoutPagina,
        UrlApi = entity.UrlApi,
        IntegraPlantao = entity.IntegraPlantao,
        AppPlantao = entity.AppPlantao,
        TrabalhaLoteGrupos = entity.TrabalhaLoteGrupos,
        PathArquivos = entity.PathArquivos,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo,
        Endereco = entity.Endereco?.ToDto(),
        Bureaux = entity.Bureaux.Select(x => x.ToDto()).ToList()
    };

    public static UsuarioDto ToDto(this Usuario entity, bool incluirClaims = true) => new()
    {
        UsuarioId = entity.UsuarioId,
        BureauId = entity.BureauId,
        Nome = entity.Nome,
        Sobrenome = entity.Sobrenome,
        Login = entity.Login,
        EmailConfirmado = entity.EmailConfirmado,
        ReadOnly = entity.ReadOnly,
        Grupo = entity.Grupo,
        Email = entity.Email,
        CodigoRecuperaSenha = entity.CodigoRecuperaSenha,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo,
        UsuarioClaims = incluirClaims ? entity.UsuarioClaims.Select(x => x.ToDto()).ToList() : new List<UsuarioClaimDto>()
    };

    public static UsuarioClaimDto ToDto(this UsuarioClaim entity) => new()
    {
        Id = entity.Id,
        UsuarioId = entity.UsuarioId,
        ClaimType = entity.ClaimType,
        ClaimValue = entity.ClaimValue
    };

    public static ProfissionalDto ToDto(this Profissional entity) => new()
    {
        ProfissionalId = entity.ProfissionalId,
        EmpresaId = entity.EmpresaId,
        Nome = entity.Nome,
        Registro = entity.Registro,
        TipoDeDocumento = entity.TipoDeDocumento,
        Cpf = entity.Cpf,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo,
        ConselhoProfissionalCodigo = entity.ConselhoProfissionalCodigo,
        UnidadeFederacaoCodigo = entity.UnidadeFederacaoCodigo,
        CboCodigo = entity.CboCodigo,
        Email = entity.Email,
        Telefone = entity.Telefone,
        Cep = entity.Cep,
        EnderecoComplemento = entity.EnderecoComplemento,
        EnderecoNumero = entity.EnderecoNumero,
        PermiteAcessoAgenda = entity.PermiteAcessoAgenda,
        AcessoPlantao = entity.AcessoPlantao,
        ComissaoGlosas = entity.ComissaoGlosas,
        NumeroPis = entity.NumeroPis,
        CodigoGrauDeParticipacao = entity.CodigoGrauDeParticipacao,
        BancoCodigo = entity.BancoCodigo,
        BancoAgencia = entity.BancoAgencia,
        BancoConta = entity.BancoConta,
        Endereco = entity.Endereco?.ToDto()
    };

    public static UnidadeHospitalarDto ToDto(this UnidadeHospitalar entity) => new()
    {
        CodigoCnes = entity.CodigoCnes,
        Cnpj = entity.Cnpj,
        Nome = entity.Nome,
        UfCodigo = entity.UfCodigo,
        UfSigla = entity.UfSigla,
        Latitude = entity.Latitude,
        Longitude = entity.Longitude,
        Description = entity.Description,
        NomeReduzido = entity.NomeReduzido,
        Cidade = entity.Cidade,
        Uf = entity.Uf,
        Token = entity.Token,
        ContatoEmail = entity.ContatoEmail,
        ContatoNome = entity.ContatoNome,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo
    };

    public static OperadoraDto ToDto(this Operadora entity) => new()
    {
        OperadoraId = entity.OperadoraId,
        OperadoraModalidadeId = entity.OperadoraModalidadeId,
        RegistroAns = entity.RegistroAns,
        RazaoSocial = entity.RazaoSocial,
        NomeFantasia = entity.NomeFantasia,
        Cnpj = entity.Cnpj,
        CarteiraMin = entity.CarteiraMin,
        CarteiraMax = entity.CarteiraMax,
        WsEndpoint = entity.WsEndpoint,
        WsRetornaBeneficiario = entity.WsRetornaBeneficiario,
        CarteiraMask = entity.CarteiraMask,
        ElegibilidadeTipo = entity.ElegibilidadeTipo,
        Token = entity.Token,
        ContatoEmail = entity.ContatoEmail,
        ContatoNome = entity.ContatoNome,
        SenhaNumCharSadt = entity.SenhaNumCharSadt,
        SenhaCharHonorario = entity.SenhaCharHonorario,
        SenhaCharConsulta = entity.SenhaCharConsulta,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo,
        EmpresaOperadora = entity.EmpresaOperadora?.ToDto()
    };

    public static EmpresaOperadoraDto ToDto(this EmpresaOperadora entity) => new()
    {
        EmpresaOperadoraId = entity.EmpresaOperadoraId,
        EmpresaId = entity.EmpresaId,
        OperadoraId = entity.OperadoraId,
        CodigoAuxiliar = entity.CodigoAuxiliar,
        CodigoNaOperadora = entity.CodigoNaOperadora,
        NumeroGuia = entity.NumeroGuia,
        Nomefantasia = entity.Nomefantasia,
        WsLogin = entity.WsLogin,
        WsSenha = entity.WsSenha,
        WsSequencia = entity.WsSequencia,
        WsHabilitado = entity.WsHabilitado,
        TrabalhaPlantao = entity.TrabalhaPlantao,
        FaturamentoTiss = entity.FaturamentoTiss,
        NumeroGuiaObrigatorio = entity.NumeroGuiaObrigatorio,
        NumeroGuiaEmpresaObrigatorio = entity.NumeroGuiaEmpresaObrigatorio,
        LiberadoParaAgendaMedica = entity.LiberadoParaAgendaMedica,
        AtendimentoUrgencia = entity.AtendimentoUrgencia,
        AcrescimoCriancaIdoso = entity.AcrescimoCriancaIdoso,
        AtendimentoHorarioEspecial = entity.AtendimentoHorarioEspecial,
        AtendimentoApartamento = entity.AtendimentoApartamento,
        SenhaGuiaObrigatorio = entity.SenhaGuiaObrigatorio,
        ValidacaoGuiaHabilitado = entity.ValidacaoGuiaHabilitado,
        LiberadoNovoFaturamento = entity.LiberadoNovoFaturamento,
        VersaoXmlEnvio = entity.VersaoXmlEnvio,
        TipoIdentificacaoNaOperadora = entity.TipoIdentificacaoNaOperadora,
        FaturamentoLiberadoBureau = entity.FaturamentoLiberadoBureau,
        CompetenciaAtiva = entity.CompetenciaAtiva,
        MaxGuiasLote = entity.MaxGuiasLote,
        AutorizadoConsulta = entity.AutorizadoConsulta,
        AutorizadoTeleConsulta = entity.AutorizadoTeleConsulta,
        ConsultaGuiaTipoId = entity.ConsultaGuiaTipoId,
        CobrancaSADTOrigem = entity.CobrancaSADTOrigem,
        CobrancaSADTSolicitante = entity.CobrancaSADTSolicitante,
        CobrancaSADTExecutante = entity.CobrancaSADTExecutante,
        CobrancaHonorarioOrigem = entity.CobrancaHonorarioOrigem,
        CobrancaHonorarioExecutante = entity.CobrancaHonorarioExecutante,
        CobrancaHonorarioContratado = entity.CobrancaHonorarioContratado,
        DiaMesFechamento = entity.DiaMesFechamento,
        MaxMesesReenvio = entity.MaxMesesReenvio,
        NumeroGuiaPrincipalObrigatorio = entity.NumeroGuiaPrincipalObrigatorio,
        CopiarGuia = entity.CopiarGuia,
        CarteiraObrigatoria = entity.CarteiraObrigatoria,
        ContratoParticular = entity.ContratoParticular,
        ConvenioPublico = entity.ConvenioPublico,
        WsMrChipUrl = entity.WsMrChipUrl,
        WsMrChipElegibilidade = entity.WsMrChipElegibilidade,
        CaraterDoAtendimentoPadrao = entity.CaraterDoAtendimentoPadrao,
        TipoAtendimentoPadrao = entity.TipoAtendimentoPadrao,
        RegimeAtendimentoPadrao = entity.RegimeAtendimentoPadrao,
        Checkin = entity.Checkin,
        Checkout = entity.Checkout,
        TrabalhaTecnica = entity.TrabalhaTecnica,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo
    };
}
