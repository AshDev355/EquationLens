using System.Data.Common;

namespace EquationLens.Data;

/// <summary>Creates database connections for the Dapper repositories.</summary>
public interface IDbConnectionFactory
{
    /// <summary>Returns a new, closed connection. The caller disposes it.</summary>
    DbConnection CreateConnection();
}
