using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Models.Adm;
using MrChip.MedLincePro.Data.Connection;

namespace MrChip.MedLincePro.Data.Repositories.Adm;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly ILogger<UsuarioRepository> _logger;

    public UsuarioRepository(ISqlConnectionFactory connectionFactory, ILogger<UsuarioRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<Usuario?> ObterPorLoginParaAutenticacaoAsync(string login, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        // Projeção preservada da consulta legada de Usuario por login.
        const string sql = @"
            SELECT
                UsuarioId, BureauId, Nome, Sobrenome, Login, PasswordHash,
                EmailConfirmado, ReadOnly, Grupo, Email, CodigoRecuperaSenha,
                DataCadastro, Ativo
            FROM dbo.Usuario
            WHERE Login = @Login;";

        return await conn.QueryFirstOrDefaultAsync<Usuario>(
            new CommandDefinition(sql, new { Login = login }, cancellationToken: ct));
    }

    public async Task<IEnumerable<UsuarioClaim>> ObterClaimsAsync(Guid usuarioId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
            SELECT Id, UsuarioId, ClaimType, ClaimValue
            FROM dbo.UsuarioClaims
            WHERE UsuarioId = @UsuarioId
            ORDER BY ClaimType, ClaimValue;";

        return await conn.QueryAsync<UsuarioClaim>(
            new CommandDefinition(sql, new { UsuarioId = usuarioId }, cancellationToken: ct));
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid usuarioId, Guid bureauId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
            SELECT
                UsuarioId, BureauId, Nome, Sobrenome, Login, PasswordHash,
                EmailConfirmado, ReadOnly, Grupo, Email, CodigoRecuperaSenha,
                DataCadastro, Ativo
            FROM dbo.Usuario
            WHERE UsuarioId = @UsuarioId
              AND BureauId = @BureauId;";

        var usuario = await conn.QueryFirstOrDefaultAsync<Usuario>(
            new CommandDefinition(sql, new { UsuarioId = usuarioId, BureauId = bureauId }, cancellationToken: ct));

        if (usuario is not null)
        {
            usuario.PasswordHash = string.Empty;
            usuario.UsuarioClaims = (await ObterClaimsAsync(usuario.UsuarioId, ct)).ToList();
        }

        return usuario;
    }

    public async Task<List<Usuario>> ListarPorBureauAsync(Guid bureauId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        const string sql = @"
            SELECT
                u.UsuarioId, u.BureauId, u.Nome, u.Sobrenome, u.Login, u.PasswordHash,
                u.EmailConfirmado, u.ReadOnly, u.Grupo, u.Email, u.CodigoRecuperaSenha,
                u.DataCadastro, u.Ativo,
                uc.Id, uc.UsuarioId, uc.ClaimType, uc.ClaimValue
            FROM dbo.Usuario u
            LEFT JOIN dbo.UsuarioClaims uc ON uc.UsuarioId = u.UsuarioId
            WHERE u.BureauId = @BureauId
            ORDER BY u.Nome, u.Sobrenome, uc.ClaimType;";

        var lookup = new Dictionary<Guid, Usuario>();

        await conn.QueryAsync<Usuario, UsuarioClaim, Usuario>(
            sql,
            (usuario, claim) =>
            {
                if (!lookup.TryGetValue(usuario.UsuarioId, out var usuarioAgregado))
                {
                    usuarioAgregado = usuario;
                    usuarioAgregado.PasswordHash = string.Empty;
                    usuarioAgregado.UsuarioClaims = new List<UsuarioClaim>();
                    lookup.Add(usuarioAgregado.UsuarioId, usuarioAgregado);
                }

                if (claim is not null && claim.Id != Guid.Empty && !usuarioAgregado.UsuarioClaims.Any(x => x.Id == claim.Id))
                    usuarioAgregado.UsuarioClaims.Add(claim);

                return usuarioAgregado;
            },
            new { BureauId = bureauId },
            splitOn: "Id");

        return lookup.Values.ToList();
    }

    public async Task<bool> CriarAsync(UsuarioCreateDto usuario, string passwordHash, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            const string sqlExiste = @"
                SELECT TOP 1 1
                FROM dbo.Usuario
                WHERE BureauId = @BureauId AND (Login = @Login OR Email = @Email);";

            var existe = await conn.ExecuteScalarAsync<int?>(new CommandDefinition(
                sqlExiste,
                new { usuario.BureauId, usuario.Login, usuario.Email },
                transaction: tx,
                cancellationToken: ct));

            if (existe.HasValue)
            {
                await tx.RollbackAsync(ct);
                return false;
            }

            const string sqlInsert = @"
                INSERT INTO dbo.Usuario
                (
                    UsuarioId, BureauId, Nome, Sobrenome, Login, PasswordHash,
                    EmailConfirmado, ReadOnly, Grupo, Email, Ativo, DataCadastro
                )
                VALUES
                (
                    @UsuarioId, @BureauId, @Nome, @Sobrenome, @Login, @PasswordHash,
                    @EmailConfirmado, @ReadOnly, @Grupo, @Email, @Ativo, @DataCadastro
                );";

            var parameters = new
            {
                UsuarioId = Guid.NewGuid(),
                usuario.BureauId,
                usuario.Nome,
                usuario.Sobrenome,
                usuario.Login,
                PasswordHash = passwordHash,
                EmailConfirmado = true,
                ReadOnly = true,
                Grupo = usuario.Grupo == 0 ? 3 : usuario.Grupo,
                usuario.Email,
                Ativo = true,
                DataCadastro = DateTime.UtcNow
            };

            var rows = await conn.ExecuteAsync(new CommandDefinition(sqlInsert, parameters, transaction: tx, cancellationToken: ct));
            if (rows > 0)
            {
                await tx.CommitAsync(ct);
                return true;
            }

            await tx.RollbackAsync(ct);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário. Login: {Login}. BureauId: {BureauId}", usuario.Login, usuario.BureauId);
            await tx.RollbackAsync(ct);
            return false;
        }
    }

    public async Task<bool> AtualizarAsync(UsuarioUpdateDto usuario, string? novoPasswordHash, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);

        var sql = string.IsNullOrWhiteSpace(novoPasswordHash)
            ? @"
                UPDATE dbo.Usuario
                   SET Nome = @Nome,
                       Sobrenome = @Sobrenome,
                       Login = @Login,
                       Email = @Email,
                       Grupo = @Grupo,
                       CodigoRecuperaSenha = @CodigoRecuperaSenha,
                       Ativo = @Ativo
                 WHERE UsuarioId = @UsuarioId
                   AND BureauId = @BureauId;"
            : @"
                UPDATE dbo.Usuario
                   SET Nome = @Nome,
                       Sobrenome = @Sobrenome,
                       Login = @Login,
                       PasswordHash = @PasswordHash,
                       Email = @Email,
                       Grupo = @Grupo,
                       CodigoRecuperaSenha = 0,
                       Ativo = @Ativo
                 WHERE UsuarioId = @UsuarioId
                   AND BureauId = @BureauId;";

        var parameters = new
        {
            usuario.UsuarioId,
            usuario.BureauId,
            usuario.Nome,
            usuario.Sobrenome,
            usuario.Login,
            PasswordHash = novoPasswordHash,
            usuario.Email,
            usuario.Grupo,
            usuario.CodigoRecuperaSenha,
            usuario.Ativo
        };

        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: ct));
        return rows > 0;
    }

    public async Task<bool> InativarAsync(Guid usuarioId, Guid bureauId, CancellationToken ct = default)
    {
        await using var conn = await _connectionFactory.OpenConnectionAsync(MedLinceDatabase.Adm, ct);
        const string sql = "UPDATE dbo.Usuario SET Ativo = 0 WHERE UsuarioId = @UsuarioId AND BureauId = @BureauId;";
        var rows = await conn.ExecuteAsync(new CommandDefinition(sql, new { UsuarioId = usuarioId, BureauId = bureauId }, cancellationToken: ct));
        return rows > 0;
    }
}
