using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace EquationLens.Data;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        // Make sure SQLite enforces foreign keys (cascade deletes) on every connection.
        _connectionString = new SqliteConnectionStringBuilder(connectionString) { ForeignKeys = true }.ToString();
    }

    public DbConnection CreateConnection() => new SqliteConnection(_connectionString);
}
