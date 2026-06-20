using Microsoft.Data.SqlClient;

namespace MrChip.MedLincePro.Data.Connection;

public interface ISqlConnectionFactory
{
    string GetConnectionString(MedLinceDatabase database);
    Task<SqlConnection> OpenConnectionAsync(MedLinceDatabase database, CancellationToken ct = default);
}
