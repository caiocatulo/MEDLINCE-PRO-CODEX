using Dapper;
using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Models.Adm;
using MrChip.MedLincePro.Data.Connection;

namespace MrChip.MedLincePro.Data.Repositories.Adm;

public sealed class UnidadeHospitalarRepository : IUnidadeHospitalarRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly ILogger<UnidadeHospitalarRepository> _logger;

    public UnidadeHospitalarRepository(ISqlConnectionFactory connectionFactory, ILogger<UnidadeHospitalarRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<UnidadeHospitalar>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
            SELECT
                u.CodigoCnes, u.Cnpj, u.Nome, u.UfCodigo, u.UfSigla, u.Latitude,
                u.Longitude, u.Description, u.NomeReduzido, u.Cidade, u.Uf, u.Token,
                u.ContatoEmail, u.ContatoNome, u.Ativo, u.DataCadastro
            FROM dbo.UnidadeHospitalar u
            INNER JOIN dbo.EmpresaUnidadeHospitalar eu ON u.CodigoCnes = eu.CodigoCnes
            WHERE eu.Ativo = 1
              AND eu.EmpresaId = @EmpresaId
            ORDER BY u.NomeReduzido;";

        return await connection.QueryAsync<UnidadeHospitalar>(
            new CommandDefinition(sql, new { EmpresaId = empresaId }, cancellationToken: ct));
    }

    public async Task<UnidadeHospitalar?> ObterPorCnesAsync(Guid empresaId, string codigoCnes, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
            SELECT
                u.CodigoCnes, u.Cnpj, u.Nome, u.UfCodigo, u.UfSigla, u.Latitude,
                u.Longitude, u.Description, u.NomeReduzido, u.Cidade, u.Uf, u.Token,
                u.ContatoEmail, u.ContatoNome, u.Ativo, u.DataCadastro
            FROM dbo.UnidadeHospitalar u
            INNER JOIN dbo.EmpresaUnidadeHospitalar eu ON u.CodigoCnes = eu.CodigoCnes
            WHERE eu.Ativo = 1
              AND eu.EmpresaId = @EmpresaId
              AND u.CodigoCnes = @CodigoCnes;";

        var unidade = await connection.QueryFirstOrDefaultAsync<UnidadeHospitalar>(
            new CommandDefinition(sql, new { EmpresaId = empresaId, CodigoCnes = codigoCnes }, cancellationToken: ct));

        if (unidade is null)
            _logger.LogInformation("Unidade hospitalar não encontrada. CodigoCnes: {CodigoCnes}, EmpresaId: {EmpresaId}", codigoCnes, empresaId);

        return unidade;
    }

    public async Task<bool> AdicionarAsync(Guid empresaId, UnidadeHospitalar unidade, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            const string sqlUpsertUnidade = @"
                IF NOT EXISTS (SELECT 1 FROM dbo.UnidadeHospitalar WHERE CodigoCnes = @CodigoCnes)
                BEGIN
                    INSERT INTO dbo.UnidadeHospitalar
                    (CodigoCnes, Cnpj, Nome, UfCodigo, UfSigla, Latitude, Longitude, Description,
                     NomeReduzido, Cidade, Uf, Token, ContatoEmail, ContatoNome, Ativo, DataCadastro)
                    VALUES
                    (@CodigoCnes, @Cnpj, @Nome, @UfCodigo, @UfSigla, @Latitude, @Longitude, @Description,
                     @NomeReduzido, @Cidade, @Uf, @Token, @ContatoEmail, @ContatoNome, @Ativo, @DataCadastro);
                END";

            await conn.ExecuteAsync(new CommandDefinition(sqlUpsertUnidade, unidade, transaction: tx, cancellationToken: ct));

            const string sqlUpsertVinculo = @"
                IF EXISTS (SELECT 1 FROM dbo.EmpresaUnidadeHospitalar WHERE EmpresaId = @EmpresaId AND CodigoCnes = @CodigoCnes)
                    UPDATE dbo.EmpresaUnidadeHospitalar
                       SET Ativo = 1,
                           Nome = @Nome
                     WHERE EmpresaId = @EmpresaId AND CodigoCnes = @CodigoCnes;
                ELSE
                    INSERT INTO dbo.EmpresaUnidadeHospitalar (Id, EmpresaId, CodigoCnes, Nome, Ativo, DataCadastro)
                    VALUES (@Id, @EmpresaId, @CodigoCnes, @Nome, 1, @DataCadastro);";

            var rows = await conn.ExecuteAsync(new CommandDefinition(
                sqlUpsertVinculo,
                new
                {
                    Id = Guid.NewGuid(),
                    EmpresaId = empresaId,
                    unidade.CodigoCnes,
                    unidade.Nome,
                    DataCadastro = DateTime.UtcNow
                },
                transaction: tx,
                cancellationToken: ct));

            await tx.CommitAsync(ct);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar unidade hospitalar. CodigoCnes: {CodigoCnes}, EmpresaId: {EmpresaId}", unidade.CodigoCnes, empresaId);
            await tx.RollbackAsync(ct);
            return false;
        }
    }

    public async Task<bool> AtualizarAsync(Guid empresaId, UnidadeHospitalar unidade, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            const string sqlUnidade = @"
                UPDATE dbo.UnidadeHospitalar
                   SET Cnpj = @Cnpj,
                       Nome = @Nome,
                       UfCodigo = @UfCodigo,
                       UfSigla = @UfSigla,
                       Latitude = @Latitude,
                       Longitude = @Longitude,
                       Description = @Description,
                       NomeReduzido = @NomeReduzido,
                       Cidade = @Cidade,
                       Uf = @Uf,
                       Token = COALESCE(@Token, Token),
                       ContatoEmail = @ContatoEmail,
                       ContatoNome = @ContatoNome,
                       Ativo = @Ativo
                 WHERE CodigoCnes = @CodigoCnes
                   AND EXISTS (
                       SELECT 1 FROM dbo.EmpresaUnidadeHospitalar eu
                       WHERE eu.EmpresaId = @EmpresaId
                         AND eu.CodigoCnes = @CodigoCnes
                   );";

            var rowsUnidade = await conn.ExecuteAsync(new CommandDefinition(sqlUnidade, new
            {
                EmpresaId = empresaId,
                unidade.CodigoCnes,
                unidade.Cnpj,
                unidade.Nome,
                unidade.UfCodigo,
                unidade.UfSigla,
                unidade.Latitude,
                unidade.Longitude,
                unidade.Description,
                unidade.NomeReduzido,
                unidade.Cidade,
                unidade.Uf,
                unidade.Token,
                unidade.ContatoEmail,
                unidade.ContatoNome,
                unidade.Ativo
            }, transaction: tx, cancellationToken: ct));

            if (rowsUnidade == 0)
            {
                await tx.RollbackAsync(ct);
                return false;
            }

            const string sqlVinculo = @"
                UPDATE dbo.EmpresaUnidadeHospitalar
                   SET Nome = @Nome,
                       Ativo = @Ativo
                 WHERE EmpresaId = @EmpresaId
                   AND CodigoCnes = @CodigoCnes;";

            await conn.ExecuteAsync(new CommandDefinition(sqlVinculo, new
            {
                EmpresaId = empresaId,
                unidade.CodigoCnes,
                unidade.Nome,
                unidade.Ativo
            }, transaction: tx, cancellationToken: ct));

            await tx.CommitAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar unidade hospitalar. CodigoCnes: {CodigoCnes}, EmpresaId: {EmpresaId}", unidade.CodigoCnes, empresaId);
            await tx.RollbackAsync(ct);
            return false;
        }
    }

    public async Task<bool> InativarVinculoAsync(Guid empresaId, string codigoCnes, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        const string sql = "UPDATE dbo.EmpresaUnidadeHospitalar SET Ativo = 0 WHERE EmpresaId = @EmpresaId AND CodigoCnes = @CodigoCnes;";
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { EmpresaId = empresaId, CodigoCnes = codigoCnes }, cancellationToken: ct));
        return rows > 0;
    }
}
