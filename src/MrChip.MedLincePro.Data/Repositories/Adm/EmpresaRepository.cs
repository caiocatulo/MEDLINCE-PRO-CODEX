using Dapper;
using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Models.Adm;
using MrChip.MedLincePro.Data.Connection;

namespace MrChip.MedLincePro.Data.Repositories.Adm;

public sealed class EmpresaRepository : IEmpresaRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly ILogger<EmpresaRepository> _logger;

    public EmpresaRepository(ISqlConnectionFactory connectionFactory, ILogger<EmpresaRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<Empresa>> ListarAsync(CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
        SELECT
            e.EmpresaId, e.RazaoSocial, e.NomeFantasia, e.Cnpj, e.Email,
            e.TelefoneDdd, e.TelefoneNumero, e.ContatoNome, e.ContatoEmail,
            e.ContatoTelefoneDdd, e.ContatoTelefoneNumero, e.Cep,
            e.EnderecoComplemento, e.EnderecoNumero, e.Cnes, e.EmpresaTipo,
            e.LogoEmpresaMaior, e.LogoEmpresaMenor, e.LogoSistemaMenor, e.LogoSistemaMaior,
            e.LayoutPagina, e.UrlApi, e.DataCadastro, e.Ativo, e.IntegraPlantao, e.AppPlantao,
            e.NumeroGuiaPrestador, e.TrabalhaLoteGrupos,
            e.PercentualPIS, e.PercentualCOFINS, e.PercentualCSLL, e.PercentualIR,
            e.PercentualINSS, e.PercentualISS, e.TaxaAdm, e.PathArquivos,
            endEmp.Cep AS EnderecoCep, endEmp.Cep, endEmp.Logradouro, endEmp.Bairro,
            endEmp.Cidade, endEmp.Uf, endEmp.DataCadastro,
            b.BureauId, b.EmpresaId, b.Nome, b.NomeResumido, b.CpfCnpj, b.Contato, b.Email,
            b.TelefoneFixo, b.TelefoneCel, b.Cep, b.EnderecoComplemento,
            b.EnderecoNumero, b.ApiAcesso, b.ApiIp, b.ApiAcessos, b.ApiCulture,
            b.ApiCountry, b.Adm, b.AcessoApi, b.PrazoInclusaoGuia, b.Ativo, b.DataCadastro
        FROM dbo.Empresa e
        INNER JOIN dbo.Endereco AS endEmp ON endEmp.Cep = e.Cep
        LEFT JOIN dbo.Bureau AS b ON b.EmpresaId = e.EmpresaId
        ORDER BY e.RazaoSocial, b.NomeResumido;";

        var lookup = new Dictionary<Guid, Empresa>();

        await conn.QueryAsync<Empresa, Endereco, Bureau, Empresa>(
            sql,
            (empresa, endereco, bureau) => MapEmpresa(lookup, empresa, endereco, bureau),
            splitOn: "EnderecoCep,BureauId");

        return lookup.Values.OrderBy(x => x.RazaoSocial).ToList();
    }

    public async Task<Empresa?> ObterPorIdAsync(Guid empresaId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
        SELECT
            e.EmpresaId, e.RazaoSocial, e.NomeFantasia, e.Cnpj, e.Email,
            e.TelefoneDdd, e.TelefoneNumero, e.ContatoNome, e.ContatoEmail,
            e.ContatoTelefoneDdd, e.ContatoTelefoneNumero, e.Cep,
            e.EnderecoComplemento, e.EnderecoNumero, e.Cnes, e.EmpresaTipo,
            e.LogoEmpresaMaior, e.LogoEmpresaMenor, e.LogoSistemaMenor, e.LogoSistemaMaior,
            e.LayoutPagina, e.UrlApi, e.DataCadastro, e.Ativo, e.IntegraPlantao, e.AppPlantao,
            e.NumeroGuiaPrestador, e.TrabalhaLoteGrupos,
            e.PercentualPIS, e.PercentualCOFINS, e.PercentualCSLL, e.PercentualIR,
            e.PercentualINSS, e.PercentualISS, e.TaxaAdm, e.PathArquivos,
            endEmp.Cep AS EnderecoSplit,
            endEmp.Cep AS Cep, endEmp.Logradouro, endEmp.Bairro, endEmp.Cidade, endEmp.Uf, endEmp.DataCadastro,
            b.BureauId, b.EmpresaId, b.Nome, b.NomeResumido, b.CpfCnpj, b.Contato, b.Email,
            b.TelefoneFixo, b.TelefoneCel, b.Cep, b.EnderecoComplemento, b.EnderecoNumero,
            b.ApiAcesso, b.ApiIp, b.ApiAcessos, b.ApiCulture, b.ApiCountry, b.Adm, b.AcessoApi,
            b.PrazoInclusaoGuia, b.Ativo, b.DataCadastro
        FROM dbo.Empresa e
        INNER JOIN dbo.Endereco AS endEmp ON endEmp.Cep = e.Cep
        LEFT JOIN dbo.Bureau AS b ON b.EmpresaId = e.EmpresaId
        WHERE e.EmpresaId = @EmpresaId
        ORDER BY e.RazaoSocial, b.NomeResumido;";

        var lookup = new Dictionary<Guid, Empresa>();

        await conn.QueryAsync<Empresa, Endereco, Bureau, Empresa>(
            sql,
            (empresa, endereco, bureau) => MapEmpresa(lookup, empresa, endereco, bureau),
            new { EmpresaId = empresaId },
            splitOn: "EnderecoSplit,BureauId");

        return lookup.Values.FirstOrDefault();
    }

    public async Task<bool> AdicionarAsync(Empresa empresa, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
        INSERT INTO dbo.Empresa
        (
            EmpresaId, RazaoSocial, NomeFantasia, Cnpj, Email, TelefoneDdd, TelefoneNumero,
            ContatoNome, ContatoEmail, ContatoTelefoneDdd, ContatoTelefoneNumero, Cep,
            EnderecoComplemento, EnderecoNumero, Cnes, EmpresaTipo, LogoEmpresaMaior,
            LogoEmpresaMenor, LogoSistemaMenor, LogoSistemaMaior, LayoutPagina, UrlApi,
            IntegraPlantao, AppPlantao, NumeroGuiaPrestador, TrabalhaLoteGrupos,
            PercentualPIS, PercentualCOFINS, PercentualCSLL, PercentualIR, PercentualINSS,
            PercentualISS, TaxaAdm, PathArquivos, DataCadastro, Ativo
        )
        VALUES
        (
            @EmpresaId, @RazaoSocial, @NomeFantasia, @Cnpj, @Email, @TelefoneDdd, @TelefoneNumero,
            @ContatoNome, @ContatoEmail, @ContatoTelefoneDdd, @ContatoTelefoneNumero, @Cep,
            @EnderecoComplemento, @EnderecoNumero, @Cnes, @EmpresaTipo, @LogoEmpresaMaior,
            @LogoEmpresaMenor, @LogoSistemaMenor, @LogoSistemaMaior, @LayoutPagina, @UrlApi,
            @IntegraPlantao, @AppPlantao, @NumeroGuiaPrestador, @TrabalhaLoteGrupos,
            @PercentualPIS, @PercentualCOFINS, @PercentualCSLL, @PercentualIR, @PercentualINSS,
            @PercentualISS, @TaxaAdm, @PathArquivos, @DataCadastro, @Ativo
        );";

        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, empresa, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<bool> AtualizarAsync(Empresa empresa, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
        UPDATE dbo.Empresa
           SET RazaoSocial             = @RazaoSocial,
               NomeFantasia            = @NomeFantasia,
               Cnpj                    = @Cnpj,
               Email                   = @Email,
               TelefoneDdd             = @TelefoneDdd,
               TelefoneNumero          = @TelefoneNumero,
               ContatoNome             = @ContatoNome,
               ContatoEmail            = @ContatoEmail,
               ContatoTelefoneDdd      = @ContatoTelefoneDdd,
               ContatoTelefoneNumero   = @ContatoTelefoneNumero,
               Cep                     = @Cep,
               EnderecoComplemento     = @EnderecoComplemento,
               EnderecoNumero          = @EnderecoNumero,
               Cnes                    = @Cnes,
               EmpresaTipo             = @EmpresaTipo,
               LogoEmpresaMaior        = @LogoEmpresaMaior,
               LogoEmpresaMenor        = @LogoEmpresaMenor,
               LogoSistemaMenor        = @LogoSistemaMenor,
               LogoSistemaMaior        = @LogoSistemaMaior,
               LayoutPagina            = @LayoutPagina,
               UrlApi                  = @UrlApi,
               AppPlantao              = @AppPlantao,
               IntegraPlantao          = @IntegraPlantao,
               NumeroGuiaPrestador     = @NumeroGuiaPrestador,
               TrabalhaLoteGrupos      = @TrabalhaLoteGrupos,
               PercentualPIS           = @PercentualPIS,
               PercentualCOFINS        = @PercentualCOFINS,
               PercentualCSLL          = @PercentualCSLL,
               PercentualIR            = @PercentualIR,
               PercentualINSS          = @PercentualINSS,
               PercentualISS           = @PercentualISS,
               TaxaAdm                 = @TaxaAdm,
               PathArquivos            = @PathArquivos,
               Ativo                   = @Ativo
         WHERE EmpresaId               = @EmpresaId;";

        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, empresa, cancellationToken: ct));
        if (rows == 0) _logger.LogWarning("Empresa não atualizada. EmpresaId: {EmpresaId}", empresa.EmpresaId);
        return rows > 0;
    }

    public async Task<bool> InativarAsync(Guid empresaId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        const string sql = "UPDATE dbo.Empresa SET Ativo = 0 WHERE EmpresaId = @EmpresaId;";
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { EmpresaId = empresaId }, cancellationToken: ct));
        return rows > 0;
    }

    private static Empresa MapEmpresa(IDictionary<Guid, Empresa> lookup, Empresa empresa, Endereco endereco, Bureau bureau)
    {
        if (!lookup.TryGetValue(empresa.EmpresaId, out var agg))
        {
            agg = empresa;
            agg.Endereco = endereco;
            agg.Bureaux = new List<Bureau>();
            lookup.Add(agg.EmpresaId, agg);
        }

        if (bureau is not null && bureau.BureauId != Guid.Empty && !agg.Bureaux.Any(x => x.BureauId == bureau.BureauId))
            agg.Bureaux.Add(bureau);

        return agg;
    }
}
