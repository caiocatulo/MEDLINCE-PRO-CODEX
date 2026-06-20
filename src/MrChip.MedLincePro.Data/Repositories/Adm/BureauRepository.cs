using Dapper;
using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Models.Adm;
using MrChip.MedLincePro.Data.Connection;

namespace MrChip.MedLincePro.Data.Repositories.Adm;

public sealed class BureauRepository : IBureauRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly ILogger<BureauRepository> _logger;

    public BureauRepository(ISqlConnectionFactory connectionFactory, ILogger<BureauRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<Bureau>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        // Projeção preservada da consulta legada de Bureau por empresa.
        const string sql = @"
            SELECT
                C.BureauId, C.EmpresaId, C.Nome, C.NomeResumido, C.CpfCnpj, C.Contato,
                C.Email, C.TelefoneFixo, C.TelefoneCel, C.Cep, C.EnderecoComplemento,
                C.EnderecoNumero, C.DataCadastro, C.Ativo, C.AcessoApi, C.ApiAcesso,
                C.ApiIp, C.ApiAcessos, C.ApiCulture, C.ApiCountry, C.Adm, C.PrazoInclusaoGuia,
                P.Cep AS EnderecoCep, P.Cep, P.Logradouro, P.Bairro, P.Cidade, P.Uf, P.DataCadastro
            FROM dbo.Bureau C
            INNER JOIN dbo.Endereco P ON C.Cep = P.Cep
            WHERE C.EmpresaId = @EmpresaId
            ORDER BY C.Nome;";

        var result = await conn.QueryAsync<Bureau, Endereco, Bureau>(
            sql,
            (bureau, endereco) => { bureau.Endereco = endereco; return bureau; },
            new { EmpresaId = empresaId },
            splitOn: "EnderecoCep");

        return result.ToList();
    }

    public async Task<Bureau?> ObterPorIdAsync(Guid bureauId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        // Projeção preservada da consulta legada de Bureau por ID.
        const string sql = @"
            SELECT
                C.BureauId, C.EmpresaId, C.Nome, C.NomeResumido, C.CpfCnpj, C.Contato,
                C.Email, C.TelefoneFixo, C.TelefoneCel, C.Cep, C.EnderecoComplemento,
                C.EnderecoNumero, C.DataCadastro, C.Ativo, C.AcessoApi, C.ApiAcesso,
                C.ApiIp, C.ApiAcessos, C.ApiCulture, C.ApiCountry, C.Adm, C.PrazoInclusaoGuia,
                P.Cep AS EnderecoCep, P.Cep, P.Logradouro, P.Bairro, P.Cidade, P.Uf, P.DataCadastro
            FROM dbo.Bureau C
            LEFT JOIN dbo.Endereco P ON C.Cep = P.Cep
            WHERE C.BureauId = @BureauId;";

        var result = await conn.QueryAsync<Bureau, Endereco, Bureau>(
            sql,
            (bureau, endereco) => { if (endereco is not null) bureau.Endereco = endereco; return bureau; },
            new { BureauId = bureauId },
            splitOn: "EnderecoCep");

        return result.FirstOrDefault();
    }

    public async Task<bool> AdicionarAsync(Bureau bureau, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
        INSERT INTO dbo.Bureau
        (
            BureauId, EmpresaId, Nome, NomeResumido, CpfCnpj, Contato, Email,
            TelefoneFixo, TelefoneCel, Cep, EnderecoComplemento, EnderecoNumero,
            DataCadastro, Ativo, AcessoApi, ApiAcesso, ApiIp, ApiAcessos,
            ApiCulture, ApiCountry, Adm, PrazoInclusaoGuia
        )
        VALUES
        (
            @BureauId, @EmpresaId, @Nome, @NomeResumido, @CpfCnpj, @Contato, @Email,
            @TelefoneFixo, @TelefoneCel, @Cep, @EnderecoComplemento, @EnderecoNumero,
            @DataCadastro, @Ativo, @AcessoApi, @ApiAcesso, @ApiIp, @ApiAcessos,
            @ApiCulture, @ApiCountry, @Adm, @PrazoInclusaoGuia
        );";

        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, bureau, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<bool> AtualizarAsync(Bureau bureau, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        // UPDATE preserva campos editáveis do legado.
        const string sql = @"
        UPDATE dbo.Bureau
           SET Nome = @Nome,
               NomeResumido = @NomeResumido,
               CpfCnpj = @CpfCnpj,
               Contato = @Contato,
               Email = @Email,
               TelefoneFixo = @TelefoneFixo,
               TelefoneCel = @TelefoneCel,
               Cep = @Cep,
               EnderecoComplemento = @EnderecoComplemento,
               EnderecoNumero = @EnderecoNumero,
               Adm = @Adm,
               PrazoInclusaoGuia = @PrazoInclusaoGuia
         WHERE BureauId = @BureauId;";

        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, bureau, cancellationToken: ct));
        if (rows == 0) _logger.LogWarning("Bureau não atualizado. BureauId: {BureauId}", bureau.BureauId);
        return rows > 0;
    }

    public async Task<bool> InativarAsync(Guid bureauId, Guid empresaId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        const string sql = "UPDATE dbo.Bureau SET Ativo = 0 WHERE BureauId = @BureauId AND EmpresaId = @EmpresaId;";
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { BureauId = bureauId, EmpresaId = empresaId }, cancellationToken: ct));
        return rows > 0;
    }
}
