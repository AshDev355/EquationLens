using Dapper;
using EquationLens.Data;
using EquationLens.Models;

namespace EquationLens.Repositories;

public class EquationRepository(IDbConnectionFactory factory) : IEquationRepository
{
    public async Task<int> AddAsync(EquationRecord equation)
    {
        const string sql = @"
INSERT INTO Equations (UserId, Equation, Result, Source, CreatedAt)
VALUES (@UserId, @Equation, @Result, @Source, @CreatedAt);
SELECT last_insert_rowid();";
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, equation);
    }

    public async Task<IReadOnlyList<EquationRecord>> GetByUserAsync(string userId, int take = 20)
    {
        const string sql = @"
SELECT Id, UserId, Equation, Result, Source, CreatedAt
FROM Equations
WHERE UserId = @userId
ORDER BY CreatedAt DESC, Id DESC
LIMIT @take;";
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync<EquationRecord>(sql, new { userId, take })).ToList();
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        using var conn = factory.CreateConnection();
        var rows = await conn.ExecuteAsync("DELETE FROM Equations WHERE Id = @id AND UserId = @userId;", new { id, userId });
        return rows > 0;
    }

    public async Task<int> DeleteAllByUserAsync(string userId)
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteAsync("DELETE FROM Equations WHERE UserId = @userId;", new { userId });
    }

    public async Task<int> CountAsync()
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Equations;");
    }
}
