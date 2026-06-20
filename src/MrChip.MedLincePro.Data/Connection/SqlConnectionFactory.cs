using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MrChip.MedLincePro.Data.Connection;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseConnectionOptions _options;

    public SqlConnectionFactory(IConfiguration configuration, IOptions<DatabaseConnectionOptions> options)
    {
        _configuration = configuration;
        _options = options.Value;
    }

    public async Task<SqlConnection> OpenConnectionAsync(MedLinceDatabase database, CancellationToken ct = default)
    {
        var connection = new SqlConnection(GetConnectionString(database));
        await connection.OpenAsync(ct);
        return connection;
    }

    public string GetConnectionString(MedLinceDatabase database)
    {
        foreach (var name in GetConnectionStringNames(database))
        {
            var configured = _configuration.GetConnectionString(name);
            if (!string.IsNullOrWhiteSpace(configured))
                return ApplyPlaceholders(configured);
        }

        throw new InvalidOperationException($"Connection string não configurada para o banco {database}.");
    }

    private static IEnumerable<string> GetConnectionStringNames(MedLinceDatabase database)
    {
        return database switch
        {
            MedLinceDatabase.Adm => new[] { "MedLinceAdm", "ConnectionStringAdm" },
            MedLinceDatabase.App => new[] { "MedLinceApp", "ConnectionStringApp" },
            MedLinceDatabase.Ans => new[] { "MedLinceAns", "ConnectionStringAns" },
            MedLinceDatabase.Serilog => new[] { "MedLinceSerilog", "ConnectionStringSerilog" },
            _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
        };
    }

    private string ApplyPlaceholders(string connectionString)
    {
        var ipConnections = GetRequired(_options.IpConnections, "DatabaseConnection:IpConnections");
        var sqlUser = GetRequired(_options.SqlUser, "DatabaseConnection:SqlUser");
        var sqlPassword = ResolveSqlPassword(ipConnections);

        var resolved = connectionString
            .Replace("{IpConnections}", ipConnections, StringComparison.OrdinalIgnoreCase)
            .Replace("{SqlUser}", sqlUser, StringComparison.OrdinalIgnoreCase)
            .Replace("{SqlUserId}", sqlUser, StringComparison.OrdinalIgnoreCase)
            .Replace("{SqlPassword}", sqlPassword, StringComparison.OrdinalIgnoreCase);

        ValidateUnresolvedPlaceholders(resolved);
        return resolved;
    }

    private string ResolveSqlPassword(string ipConnections)
    {
        var envByIp = Environment.GetEnvironmentVariable(BuildSqlPasswordEnvironmentVariableName(ipConnections));
        if (!string.IsNullOrWhiteSpace(envByIp)) return envByIp;

        var genericEnv = Environment.GetEnvironmentVariable("MEDLINCE_SQL_PASSWORD");
        if (!string.IsNullOrWhiteSpace(genericEnv)) return genericEnv;

        if (!string.IsNullOrWhiteSpace(_options.SqlPassword) && !IsPlaceholder(_options.SqlPassword))
            return _options.SqlPassword;

        throw new InvalidOperationException("Senha SQL não configurada. Use variável de ambiente MEDLINCE_SQL_PASSWORD_<IP_NORMALIZADO> ou MEDLINCE_SQL_PASSWORD.");
    }

    private static string BuildSqlPasswordEnvironmentVariableName(string ipConnections)
    {
        var normalizedIp = ipConnections.Trim().Replace('.', '_').Replace('-', '_').Replace(':', '_');
        return $"MEDLINCE_SQL_PASSWORD_{normalizedIp}";
    }

    private static string GetRequired(string value, string key)
    {
        if (!string.IsNullOrWhiteSpace(value) && !IsPlaceholder(value))
            return value.Trim();

        throw new InvalidOperationException($"Configuração obrigatória não informada: {key}.");
    }

    private static void ValidateUnresolvedPlaceholders(string connectionString)
    {
        var unresolved = new[] { "{IpConnections}", "{SqlUser}", "{SqlUserId}", "{SqlPassword}" };
        foreach (var placeholder in unresolved)
        {
            if (connectionString.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Connection string contém placeholder não resolvido: {placeholder}.");
        }
    }

    private static bool IsPlaceholder(string value) =>
        value.StartsWith('{') && value.EndsWith('}');
}
