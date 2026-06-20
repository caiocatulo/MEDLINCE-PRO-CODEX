using Dapper;
using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Models.Adm;
using MrChip.MedLincePro.Data.Connection;

namespace MrChip.MedLincePro.Data.Repositories.Adm;

public sealed class OperadoraRepository : IOperadoraRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly ILogger<OperadoraRepository> _logger;

    public OperadoraRepository(ISqlConnectionFactory connectionFactory, ILogger<OperadoraRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<Operadora>> ListarPorEmpresaAsync(Guid empresaId, bool liberadoParaAgendaMedica = false, bool somenteComWsHabilitado = false, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
            SELECT
                o.OperadoraId, o.OperadoraModalidadeId, o.RegistroAns, o.RazaoSocial,
                o.NomeFantasia, o.Cnpj, o.CarteiraMin, o.CarteiraMax, o.WsEndpoint,
                o.WsRetornaBeneficiario, o.CarteiraMask, o.ElegibilidadeTipo, o.Token,
                o.ContatoEmail, o.ContatoNome, o.SenhaNumCharSadt, o.SenhaCharHonorario,
                o.SenhaCharConsulta, o.DataCadastro, o.Ativo,
                eo.EmpresaOperadoraId, eo.EmpresaId, eo.OperadoraId, eo.CodigoAuxiliar,
                eo.CodigoNaOperadora, eo.NumeroGuia, eo.NomeFantasia, eo.WsLogin,
                eo.WsSenha, eo.WsSequencia, eo.WsHabilitado, eo.TrabalhaPlantao,
                eo.FaturamentoTiss, eo.NumeroGuiaObrigatorio, eo.NumeroGuiaEmpresaObrigatorio,
                eo.LiberadoParaAgendaMedica, eo.AtendimentoUrgencia, eo.AcrescimoCriancaIdoso,
                eo.AtendimentoHorarioEspecial, eo.AtendimentoApartamento, eo.SenhaGuiaObrigatorio,
                eo.ValidacaoGuiaHabilitado, eo.LiberadoNovoFaturamento, eo.VersaoXmlEnvio,
                eo.TipoIdentificacaoNaOperadora, eo.FaturamentoLiberadoBureau, eo.CompetenciaAtiva,
                eo.MaxGuiasLote, eo.AutorizadoConsulta, eo.AutorizadoTeleConsulta,
                eo.ConsultaGuiaTipoId, eo.CobrancaSADTOrigem, eo.CobrancaSADTSolicitante,
                eo.CobrancaSADTExecutante, eo.CobrancaHonorarioOrigem, eo.CobrancaHonorarioContratado,
                eo.CobrancaHonorarioExecutante, eo.NumeroGuiaPrincipalObrigatorio, eo.CopiarGuia,
                eo.carteiraObrigatoria AS CarteiraObrigatoria, eo.ContratoParticular, eo.ConvenioPublico,
                eo.WsMrChipUrl, eo.WsMrChipElegibilidade, eo.CaraterDoAtendimentoPadrao,
                eo.TipoAtendimentoPadrao, eo.RegimeAtendimentoPadrao, eo.Checkin, eo.Checkout,
                eo.TrabalhaTecnica, eo.TagNumeroGuiaPrestador, eo.HorarioObrigatorioXml,
                eo.FormaCalculoProcedimento, eo.ModeloGuiaAuxiliar, eo.ConcatenarGrupoProcedimento,
                eo.DataCadastro, eo.Ativo
            FROM dbo.Operadora o
            INNER JOIN dbo.EmpresaOperadora eo ON eo.OperadoraId = o.OperadoraId
            WHERE eo.EmpresaId = @EmpresaId
              AND o.Ativo = 1
              AND eo.Ativo = 1
              AND (@LiberadoParaAgendaMedica = 0 OR eo.LiberadoParaAgendaMedica = 1)
              AND (@SomenteComWsHabilitado = 0 OR eo.WsHabilitado = 1)
            ORDER BY o.RazaoSocial;";

        var dados = await conn.QueryAsync<Operadora, EmpresaOperadora, Operadora>(
            sql,
            (operadora, empresaOperadora) =>
            {
                operadora.EmpresaOperadora = empresaOperadora;
                return operadora;
            },
            new { EmpresaId = empresaId, LiberadoParaAgendaMedica = liberadoParaAgendaMedica, SomenteComWsHabilitado = somenteComWsHabilitado },
            splitOn: "EmpresaOperadoraId");

        return dados.ToList();
    }

    public async Task<Operadora?> ObterPorEmpresaAsync(Guid empresaId, Guid operadoraId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
            SELECT
                o.OperadoraId, o.OperadoraModalidadeId, o.RegistroAns, o.RazaoSocial,
                o.NomeFantasia, o.Cnpj, o.CarteiraMin, o.CarteiraMax, o.WsEndpoint,
                o.WsRetornaBeneficiario, o.CarteiraMask, o.ElegibilidadeTipo, o.Token,
                o.ContatoEmail, o.ContatoNome, o.SenhaNumCharSadt, o.SenhaCharHonorario,
                o.SenhaCharConsulta, o.DataCadastro, o.Ativo,
                eo.EmpresaOperadoraId, eo.EmpresaId, eo.OperadoraId, eo.CodigoAuxiliar,
                eo.CodigoNaOperadora, eo.NumeroGuia, eo.NomeFantasia, eo.WsLogin,
                eo.WsSenha, eo.WsSequencia, eo.WsHabilitado, eo.TrabalhaPlantao,
                eo.FaturamentoTiss, eo.NumeroGuiaObrigatorio, eo.NumeroGuiaEmpresaObrigatorio,
                eo.LiberadoParaAgendaMedica, eo.AtendimentoUrgencia, eo.AcrescimoCriancaIdoso,
                eo.AtendimentoHorarioEspecial, eo.AtendimentoApartamento, eo.SenhaGuiaObrigatorio,
                eo.ValidacaoGuiaHabilitado, eo.LiberadoNovoFaturamento, eo.VersaoXmlEnvio,
                eo.TipoIdentificacaoNaOperadora, eo.FaturamentoLiberadoBureau, eo.CompetenciaAtiva,
                eo.MaxGuiasLote, eo.AutorizadoConsulta, eo.AutorizadoTeleConsulta,
                eo.ConsultaGuiaTipoId, eo.CobrancaSADTOrigem, eo.CobrancaSADTSolicitante,
                eo.CobrancaSADTExecutante, eo.CobrancaHonorarioOrigem, eo.CobrancaHonorarioContratado,
                eo.CobrancaHonorarioExecutante, eo.NumeroGuiaPrincipalObrigatorio, eo.CopiarGuia,
                eo.carteiraObrigatoria AS CarteiraObrigatoria, eo.ContratoParticular, eo.ConvenioPublico,
                eo.WsMrChipUrl, eo.WsMrChipElegibilidade, eo.CaraterDoAtendimentoPadrao,
                eo.TipoAtendimentoPadrao, eo.RegimeAtendimentoPadrao, eo.Checkin, eo.Checkout,
                eo.TrabalhaTecnica, eo.TagNumeroGuiaPrestador, eo.HorarioObrigatorioXml,
                eo.FormaCalculoProcedimento, eo.ModeloGuiaAuxiliar, eo.ConcatenarGrupoProcedimento,
                eo.DataCadastro, eo.Ativo
            FROM dbo.Operadora o
            INNER JOIN dbo.EmpresaOperadora eo ON eo.OperadoraId = o.OperadoraId
            WHERE eo.EmpresaId = @EmpresaId
              AND o.OperadoraId = @OperadoraId;";

        var dados = await conn.QueryAsync<Operadora, EmpresaOperadora, Operadora>(
            sql,
            (operadora, empresaOperadora) =>
            {
                operadora.EmpresaOperadora = empresaOperadora;
                return operadora;
            },
            new { EmpresaId = empresaId, OperadoraId = operadoraId },
            splitOn: "EmpresaOperadoraId");

        return dados.FirstOrDefault();
    }

    public async Task<bool> AdicionarAsync(Guid empresaId, Operadora operadora, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            const string sqlOperadoraExiste = "SELECT TOP 1 1 FROM dbo.Operadora WHERE OperadoraId = @OperadoraId;";
            var operadoraExiste = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(
                sqlOperadoraExiste,
                new { operadora.OperadoraId },
                transaction: tx,
                cancellationToken: ct));

            if (!operadoraExiste.HasValue)
            {
                const string sqlOperadora = @"
                    INSERT INTO dbo.Operadora
                    (
                        OperadoraId, OperadoraModalidadeId, RegistroAns, RazaoSocial, NomeFantasia,
                        Cnpj, CarteiraMin, CarteiraMax, WsEndpoint, WsRetornaBeneficiario,
                        CarteiraMask, ElegibilidadeTipo, Token, ContatoEmail, ContatoNome,
                        SenhaNumCharSadt, SenhaCharHonorario, SenhaCharConsulta, DataCadastro, Ativo
                    )
                    VALUES
                    (
                        @OperadoraId, @OperadoraModalidadeId, @RegistroAns, @RazaoSocial, @NomeFantasia,
                        @Cnpj, @CarteiraMin, @CarteiraMax, @WsEndpoint, @WsRetornaBeneficiario,
                        @CarteiraMask, @ElegibilidadeTipo, @Token, @ContatoEmail, @ContatoNome,
                        @SenhaNumCharSadt, @SenhaCharHonorario, @SenhaCharConsulta, @DataCadastro, @Ativo
                    );";

                await conn.ExecuteAsync(new CommandDefinition(sqlOperadora, operadora, transaction: tx, cancellationToken: ct));
            }

            const string sqlVinculoExiste = @"
                SELECT TOP 1 1
                FROM dbo.EmpresaOperadora
                WHERE EmpresaId = @EmpresaId AND OperadoraId = @OperadoraId;";

            var vinculoExiste = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(
                sqlVinculoExiste,
                new { EmpresaId = empresaId, operadora.OperadoraId },
                transaction: tx,
                cancellationToken: ct));

            if (vinculoExiste.HasValue)
            {
                await tx.RollbackAsync(ct);
                return false;
            }

            const string sqlEmpresaOperadora = @"
                INSERT INTO dbo.EmpresaOperadora
                (
                    EmpresaOperadoraId, EmpresaId, OperadoraId, CodigoAuxiliar, CodigoNaOperadora,
                    NumeroGuia, NomeFantasia, WsLogin, WsSenha, WsSequencia, WsHabilitado,
                    TrabalhaPlantao, FaturamentoTiss, NumeroGuiaObrigatorio,
                    NumeroGuiaEmpresaObrigatorio, LiberadoParaAgendaMedica, AtendimentoUrgencia,
                    AcrescimoCriancaIdoso, AtendimentoHorarioEspecial, AtendimentoApartamento,
                    SenhaGuiaObrigatorio, ValidacaoGuiaHabilitado, LiberadoNovoFaturamento,
                    VersaoXmlEnvio, TipoIdentificacaoNaOperadora, FaturamentoLiberadoBureau,
                    CompetenciaAtiva, MaxGuiasLote, AutorizadoConsulta, AutorizadoTeleConsulta,
                    ConsultaGuiaTipoId, CobrancaSADTOrigem, CobrancaSADTSolicitante,
                    CobrancaSADTExecutante, CobrancaHonorarioOrigem, CobrancaHonorarioContratado,
                    CobrancaHonorarioExecutante, NumeroGuiaPrincipalObrigatorio, CopiarGuia,
                    carteiraObrigatoria, ContratoParticular, ConvenioPublico, WsMrChipUrl,
                    WsMrChipElegibilidade, CaraterDoAtendimentoPadrao, TipoAtendimentoPadrao,
                    RegimeAtendimentoPadrao, Checkin, Checkout, TrabalhaTecnica,
                    TagNumeroGuiaPrestador, HorarioObrigatorioXml, FormaCalculoProcedimento,
                    ModeloGuiaAuxiliar, ConcatenarGrupoProcedimento, DataCadastro, Ativo
                )
                VALUES
                (
                    @EmpresaOperadoraId, @EmpresaId, @OperadoraId, @CodigoAuxiliar, @CodigoNaOperadora,
                    @NumeroGuia, @NomeFantasia, @WsLogin, @WsSenha, @WsSequencia, @WsHabilitado,
                    @TrabalhaPlantao, @FaturamentoTiss, @NumeroGuiaObrigatorio,
                    @NumeroGuiaEmpresaObrigatorio, @LiberadoParaAgendaMedica, @AtendimentoUrgencia,
                    @AcrescimoCriancaIdoso, @AtendimentoHorarioEspecial, @AtendimentoApartamento,
                    @SenhaGuiaObrigatorio, @ValidacaoGuiaHabilitado, @LiberadoNovoFaturamento,
                    @VersaoXmlEnvio, @TipoIdentificacaoNaOperadora, @FaturamentoLiberadoBureau,
                    @CompetenciaAtiva, @MaxGuiasLote, @AutorizadoConsulta, @AutorizadoTeleConsulta,
                    @ConsultaGuiaTipoId, @CobrancaSADTOrigem, @CobrancaSADTSolicitante,
                    @CobrancaSADTExecutante, @CobrancaHonorarioOrigem, @CobrancaHonorarioContratado,
                    @CobrancaHonorarioExecutante, @NumeroGuiaPrincipalObrigatorio, @CopiarGuia,
                    @CarteiraObrigatoria, @ContratoParticular, @ConvenioPublico, @WsMrChipUrl,
                    @WsMrChipElegibilidade, @CaraterDoAtendimentoPadrao, @TipoAtendimentoPadrao,
                    @RegimeAtendimentoPadrao, @Checkin, @Checkout, @TrabalhaTecnica,
                    @TagNumeroGuiaPrestador, @HorarioObrigatorioXml, @FormaCalculoProcedimento,
                    @ModeloGuiaAuxiliar, @ConcatenarGrupoProcedimento, @DataCadastro, @Ativo
                );";

            var vinculo = operadora.EmpresaOperadora;
            vinculo.EmpresaId = empresaId;
            vinculo.OperadoraId = operadora.OperadoraId;

            var rows = await conn.ExecuteAsync(new CommandDefinition(sqlEmpresaOperadora, vinculo, transaction: tx, cancellationToken: ct));

            await tx.CommitAsync(ct);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar/vincular operadora. EmpresaId: {EmpresaId}. OperadoraId: {OperadoraId}", empresaId, operadora.OperadoraId);
            await tx.RollbackAsync(ct);
            return false;
        }
    }

    public async Task<bool> AtualizarAsync(Guid empresaId, Operadora operadora, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            const string sqlOperadora = @"
                UPDATE dbo.Operadora
                   SET OperadoraModalidadeId = @OperadoraModalidadeId,
                       RegistroAns = @RegistroAns,
                       RazaoSocial = @RazaoSocial,
                       NomeFantasia = @NomeFantasia,
                       Cnpj = @Cnpj,
                       CarteiraMin = @CarteiraMin,
                       CarteiraMax = @CarteiraMax,
                       WsEndpoint = @WsEndpoint,
                       WsRetornaBeneficiario = @WsRetornaBeneficiario,
                       CarteiraMask = @CarteiraMask,
                       ElegibilidadeTipo = @ElegibilidadeTipo,
                       Token = COALESCE(@Token, Token),
                       ContatoEmail = @ContatoEmail,
                       ContatoNome = @ContatoNome,
                       SenhaNumCharSadt = @SenhaNumCharSadt,
                       SenhaCharHonorario = @SenhaCharHonorario,
                       SenhaCharConsulta = @SenhaCharConsulta,
                       Ativo = @Ativo
                 WHERE OperadoraId = @OperadoraId;";

            var rowsOperadora = await conn.ExecuteAsync(new CommandDefinition(sqlOperadora, operadora, transaction: tx, cancellationToken: ct));

            const string sqlEmpresaOperadora = @"
                UPDATE dbo.EmpresaOperadora
                   SET CodigoAuxiliar = @CodigoAuxiliar,
                       CodigoNaOperadora = @CodigoNaOperadora,
                       NumeroGuia = @NumeroGuia,
                       NomeFantasia = @NomeFantasia,
                       WsLogin = @WsLogin,
                       WsSenha = COALESCE(@WsSenha, WsSenha),
                       WsSequencia = @WsSequencia,
                       WsHabilitado = @WsHabilitado,
                       TrabalhaPlantao = @TrabalhaPlantao,
                       FaturamentoTiss = @FaturamentoTiss,
                       NumeroGuiaObrigatorio = @NumeroGuiaObrigatorio,
                       NumeroGuiaEmpresaObrigatorio = @NumeroGuiaEmpresaObrigatorio,
                       LiberadoParaAgendaMedica = @LiberadoParaAgendaMedica,
                       AtendimentoUrgencia = @AtendimentoUrgencia,
                       AcrescimoCriancaIdoso = @AcrescimoCriancaIdoso,
                       AtendimentoHorarioEspecial = @AtendimentoHorarioEspecial,
                       AtendimentoApartamento = @AtendimentoApartamento,
                       SenhaGuiaObrigatorio = @SenhaGuiaObrigatorio,
                       ValidacaoGuiaHabilitado = @ValidacaoGuiaHabilitado,
                       LiberadoNovoFaturamento = @LiberadoNovoFaturamento,
                       VersaoXmlEnvio = @VersaoXmlEnvio,
                       TipoIdentificacaoNaOperadora = @TipoIdentificacaoNaOperadora,
                       FaturamentoLiberadoBureau = @FaturamentoLiberadoBureau,
                       CompetenciaAtiva = @CompetenciaAtiva,
                       MaxGuiasLote = @MaxGuiasLote,
                       AutorizadoConsulta = @AutorizadoConsulta,
                       AutorizadoTeleConsulta = @AutorizadoTeleConsulta,
                       ConsultaGuiaTipoId = @ConsultaGuiaTipoId,
                       CobrancaSADTOrigem = @CobrancaSADTOrigem,
                       CobrancaSADTSolicitante = @CobrancaSADTSolicitante,
                       CobrancaSADTExecutante = @CobrancaSADTExecutante,
                       CobrancaHonorarioOrigem = @CobrancaHonorarioOrigem,
                       CobrancaHonorarioContratado = @CobrancaHonorarioContratado,
                       CobrancaHonorarioExecutante = @CobrancaHonorarioExecutante,
                       NumeroGuiaPrincipalObrigatorio = @NumeroGuiaPrincipalObrigatorio,
                       CopiarGuia = @CopiarGuia,
                       carteiraObrigatoria = @CarteiraObrigatoria,
                       ContratoParticular = @ContratoParticular,
                       ConvenioPublico = @ConvenioPublico,
                       WsMrChipUrl = @WsMrChipUrl,
                       WsMrChipElegibilidade = @WsMrChipElegibilidade,
                       CaraterDoAtendimentoPadrao = @CaraterDoAtendimentoPadrao,
                       TipoAtendimentoPadrao = @TipoAtendimentoPadrao,
                       RegimeAtendimentoPadrao = @RegimeAtendimentoPadrao,
                       Checkin = @Checkin,
                       Checkout = @Checkout,
                       TrabalhaTecnica = @TrabalhaTecnica,
                       TagNumeroGuiaPrestador = @TagNumeroGuiaPrestador,
                       HorarioObrigatorioXml = @HorarioObrigatorioXml,
                       FormaCalculoProcedimento = @FormaCalculoProcedimento,
                       ModeloGuiaAuxiliar = @ModeloGuiaAuxiliar,
                       ConcatenarGrupoProcedimento = @ConcatenarGrupoProcedimento,
                       Ativo = @Ativo
                 WHERE EmpresaId = @EmpresaId
                   AND OperadoraId = @OperadoraId;";

            var vinculo = operadora.EmpresaOperadora;
            vinculo.EmpresaId = empresaId;
            vinculo.OperadoraId = operadora.OperadoraId;
            var rowsVinculo = await conn.ExecuteAsync(new CommandDefinition(sqlEmpresaOperadora, vinculo, transaction: tx, cancellationToken: ct));

            await tx.CommitAsync(ct);
            return rowsOperadora > 0 && rowsVinculo > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar operadora. EmpresaId: {EmpresaId}. OperadoraId: {OperadoraId}", empresaId, operadora.OperadoraId);
            await tx.RollbackAsync(ct);
            return false;
        }
    }

    public async Task<bool> InativarVinculoAsync(Guid empresaId, Guid operadoraId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
            UPDATE dbo.EmpresaOperadora
               SET Ativo = 0
             WHERE EmpresaId = @EmpresaId
               AND OperadoraId = @OperadoraId;";

        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { EmpresaId = empresaId, OperadoraId = operadoraId }, cancellationToken: ct));
        return rows > 0;
    }
}
