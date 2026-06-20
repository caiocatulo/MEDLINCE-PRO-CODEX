using Dapper;
using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Models.Adm;
using MrChip.MedLincePro.Data.Connection;

namespace MrChip.MedLincePro.Data.Repositories.Adm;

public sealed class ProfissionalRepository : IProfissionalRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly ILogger<ProfissionalRepository> _logger;

    public ProfissionalRepository(ISqlConnectionFactory connectionFactory, ILogger<ProfissionalRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<Profissional>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        // Projeção baseada na consulta legada de Profissional por Empresa.
        const string sql = @"
            SELECT
                P.ProfissionalId, P.EmpresaId, P.Nome, P.Registro, P.TipoDeDocumento,
                P.Cpf, P.DataCadastro, P.Ativo, P.ConselhoProfissionalCodigo,
                P.UnidadeFederacaoCodigo, P.CboCodigo, P.Email, P.Telefone, P.Cep,
                P.EnderecoComplemento, P.EnderecoNumero, P.PermiteAcessoAgenda,
                P.AcessoPlantao, P.ComissaoGlosas, P.NumeroPis, P.CodigoGrauDeParticipacao,
                P.BancoCodigo, P.BancoAgencia, P.BancoConta,
                E.Cep AS EnderecoCep, E.Cep, E.Logradouro, E.Bairro, E.Cidade, E.Uf, E.DataCadastro
            FROM dbo.Profissional P
            LEFT JOIN dbo.Endereco E ON E.Cep = P.Cep
            WHERE P.EmpresaId = @EmpresaId
            ORDER BY P.Nome;";

        var result = await conn.QueryAsync<Profissional, Endereco, Profissional>(
            sql,
            (profissional, endereco) => { if (endereco is not null) profissional.Endereco = endereco; return profissional; },
            new { EmpresaId = empresaId },
            splitOn: "EnderecoCep");

        return result.ToList();
    }

    public async Task<Profissional?> ObterPorIdAsync(Guid empresaId, Guid profissionalId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        // Projeção preservada do padrão legado de Profissional por ID.
        const string sql = @"
            SELECT
                P.ProfissionalId, P.EmpresaId, P.Nome, P.Registro, P.TipoDeDocumento,
                P.Cpf, P.DataCadastro, P.Ativo, P.ConselhoProfissionalCodigo,
                P.UnidadeFederacaoCodigo, P.CboCodigo, P.Email, P.Telefone, P.Cep,
                P.EnderecoComplemento, P.EnderecoNumero, P.PermiteAcessoAgenda,
                P.AcessoPlantao, P.ComissaoGlosas, P.NumeroPis, P.CodigoGrauDeParticipacao,
                P.BancoCodigo, P.BancoAgencia, P.BancoConta,
                E.Cep AS EnderecoCep, E.Cep, E.Logradouro, E.Bairro, E.Cidade, E.Uf, E.DataCadastro
            FROM dbo.Profissional P
            LEFT JOIN dbo.Endereco E ON E.Cep = P.Cep
            WHERE P.ProfissionalId = @ProfissionalId
              AND P.EmpresaId = @EmpresaId;";

        var result = await conn.QueryAsync<Profissional, Endereco, Profissional>(
            sql,
            (profissional, endereco) => { if (endereco is not null) profissional.Endereco = endereco; return profissional; },
            new { EmpresaId = empresaId, ProfissionalId = profissionalId },
            splitOn: "EnderecoCep");

        return result.FirstOrDefault();
    }

    public async Task<bool> AdicionarAsync(Profissional profissional, Guid bureauId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            const string sqlExisteCpf = @"
                SELECT TOP 1 1
                FROM dbo.Profissional
                WHERE EmpresaId = @EmpresaId
                  AND Cpf = @Cpf;";

            const string sqlExisteRegistro = @"
                SELECT TOP 1 1
                FROM dbo.Profissional
                WHERE EmpresaId = @EmpresaId
                  AND Registro = @Registro;";

            var existeCpf = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sqlExisteCpf, profissional, transaction: tx, cancellationToken: ct));
            if (existeCpf.HasValue)
            {
                await tx.RollbackAsync(ct);
                return false;
            }

            var existeRegistro = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(sqlExisteRegistro, profissional, transaction: tx, cancellationToken: ct));
            if (existeRegistro.HasValue)
            {
                await tx.RollbackAsync(ct);
                return false;
            }

            const string sqlInserirProfissional = @"
                INSERT INTO dbo.Profissional
                (
                    ProfissionalId, EmpresaId, Nome, Registro, TipoDeDocumento, Cpf,
                    DataCadastro, Ativo, ConselhoProfissionalCodigo, UnidadeFederacaoCodigo,
                    CboCodigo, Email, Telefone, Cep, EnderecoComplemento, EnderecoNumero,
                    PermiteAcessoAgenda, AcessoPlantao, ComissaoGlosas, NumeroPis,
                    CodigoGrauDeParticipacao, BancoCodigo, BancoAgencia, BancoConta
                )
                VALUES
                (
                    @ProfissionalId, @EmpresaId, @Nome, @Registro, @TipoDeDocumento, @Cpf,
                    @DataCadastro, @Ativo, @ConselhoProfissionalCodigo, @UnidadeFederacaoCodigo,
                    @CboCodigo, @Email, @Telefone, @Cep, @EnderecoComplemento, @EnderecoNumero,
                    @PermiteAcessoAgenda, @AcessoPlantao, @ComissaoGlosas, @NumeroPis,
                    @CodigoGrauDeParticipacao, @BancoCodigo, @BancoAgencia, @BancoConta
                );";

            var rows = await conn.ExecuteAsync(new CommandDefinition(sqlInserirProfissional, profissional, transaction: tx, cancellationToken: ct));

            const string sqlInserirVinculo = @"
                INSERT INTO dbo.BureauProfissional
                (
                    BureauId, ProfissionalId, Matricula, Agregado, ColetarGuia, Ativo, DataCadastro
                )
                VALUES
                (
                    @BureauId, @ProfissionalId, @Matricula, @Agregado, @ColetarGuia, @Ativo, @DataCadastro
                );";

            await conn.ExecuteAsync(new CommandDefinition(
                sqlInserirVinculo,
                new
                {
                    BureauId = bureauId,
                    profissional.ProfissionalId,
                    Matricula = string.IsNullOrWhiteSpace(profissional.Cpf) ? profissional.Registro : profissional.Cpf,
                    Agregado = true,
                    ColetarGuia = 1,
                    Ativo = true,
                    DataCadastro = DateTime.UtcNow
                },
                transaction: tx,
                cancellationToken: ct));

            await tx.CommitAsync(ct);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar profissional. ProfissionalId: {ProfissionalId}", profissional.ProfissionalId);
            await tx.RollbackAsync(ct);
            return false;
        }
    }

    public async Task<bool> AtualizarAsync(Profissional profissional, Guid bureauId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            // UPDATE preserva campos editáveis do legado.
            const string sqlUpdate = @"
                UPDATE dbo.Profissional
                   SET Nome = @Nome,
                       Registro = @Registro,
                       Cpf = @Cpf,
                       Ativo = @Ativo,
                       ConselhoProfissionalCodigo = @ConselhoProfissionalCodigo,
                       UnidadeFederacaoCodigo = @UnidadeFederacaoCodigo,
                       CboCodigo = @CboCodigo,
                       Email = @Email,
                       Telefone = @Telefone,
                       Cep = @Cep,
                       EnderecoComplemento = @EnderecoComplemento,
                       EnderecoNumero = @EnderecoNumero,
                       PermiteAcessoAgenda = @PermiteAcessoAgenda,
                       AcessoPlantao = @AcessoPlantao,
                       ComissaoGlosas = @ComissaoGlosas,
                       NumeroPis = @NumeroPis,
                       CodigoGrauDeParticipacao = @CodigoGrauDeParticipacao,
                       BancoCodigo = @BancoCodigo,
                       BancoAgencia = @BancoAgencia,
                       BancoConta = @BancoConta
                 WHERE ProfissionalId = @ProfissionalId
                   AND EmpresaId = @EmpresaId;";

            var rows = await conn.ExecuteAsync(new CommandDefinition(sqlUpdate, profissional, transaction: tx, cancellationToken: ct));
            if (rows == 0)
            {
                await tx.RollbackAsync(ct);
                return false;
            }

            const string sqlExisteVinculo = @"
                SELECT TOP 1 1
                FROM dbo.BureauProfissional
                WHERE BureauId = @BureauId
                  AND ProfissionalId = @ProfissionalId;";

            var existe = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(
                sqlExisteVinculo,
                new { BureauId = bureauId, profissional.ProfissionalId },
                transaction: tx,
                cancellationToken: ct));

            if (!existe.HasValue)
            {
                const string sqlInserirVinculo = @"
                    INSERT INTO dbo.BureauProfissional
                    (BureauId, ProfissionalId, Matricula, Agregado, ColetarGuia, Ativo, DataCadastro)
                    VALUES
                    (@BureauId, @ProfissionalId, @Matricula, @Agregado, @ColetarGuia, @Ativo, @DataCadastro);";

                await conn.ExecuteAsync(new CommandDefinition(
                    sqlInserirVinculo,
                    new
                    {
                        BureauId = bureauId,
                        profissional.ProfissionalId,
                        Matricula = string.IsNullOrWhiteSpace(profissional.Cpf) ? profissional.Registro : profissional.Cpf,
                        Agregado = true,
                        ColetarGuia = 1,
                        Ativo = true,
                        DataCadastro = DateTime.UtcNow
                    },
                    transaction: tx,
                    cancellationToken: ct));
            }

            await tx.CommitAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar profissional. ProfissionalId: {ProfissionalId}", profissional.ProfissionalId);
            await tx.RollbackAsync(ct);
            return false;
        }
    }

    public async Task<bool> InativarAsync(Guid empresaId, Guid profissionalId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        const string sql = "UPDATE dbo.Profissional SET Ativo = 0 WHERE EmpresaId = @EmpresaId AND ProfissionalId = @ProfissionalId;";
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { EmpresaId = empresaId, ProfissionalId = profissionalId }, cancellationToken: ct));
        return rows > 0;
    }
}
