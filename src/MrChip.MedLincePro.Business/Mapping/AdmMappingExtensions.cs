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
        DataCadastro = entity.DataCadastro
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
        Adm = entity.Adm,
        AcessoApi = entity.AcessoApi,
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
        NumeroGuiaPrestador = entity.NumeroGuiaPrestador,
        TrabalhaLoteGrupos = entity.TrabalhaLoteGrupos,
        PercentualPIS = entity.PercentualPIS,
        PercentualCOFINS = entity.PercentualCOFINS,
        PercentualCSLL = entity.PercentualCSLL,
        PercentualIR = entity.PercentualIR,
        PercentualINSS = entity.PercentualINSS,
        PercentualISS = entity.PercentualISS,
        TaxaAdm = entity.TaxaAdm,
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
        TipoDeDocumento = entity.TipoDeDocumento,
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
        Token = null,
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
        Token = null,
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
        NomeFantasia = entity.NomeFantasia,
        WsLogin = entity.WsLogin,
        WsSenha = null,
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
        CobrancaHonorarioContratado = entity.CobrancaHonorarioContratado,
        CobrancaHonorarioExecutante = entity.CobrancaHonorarioExecutante,
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
        TagNumeroGuiaPrestador = entity.TagNumeroGuiaPrestador,
        HorarioObrigatorioXml = entity.HorarioObrigatorioXml,
        FormaCalculoProcedimento = entity.FormaCalculoProcedimento,
        ModeloGuiaAuxiliar = entity.ModeloGuiaAuxiliar,
        ConcatenarGrupoProcedimento = entity.ConcatenarGrupoProcedimento,
        DataCadastro = entity.DataCadastro,
        Ativo = entity.Ativo
    };
}
