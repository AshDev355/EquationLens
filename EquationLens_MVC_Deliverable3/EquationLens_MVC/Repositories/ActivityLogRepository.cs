using Dapper;
using EquationLens.Data;
using EquationLens.Models;

namespace EquationLens.Repositories;

public class ActivityLogRepository(IDbConnectionFactory factory) : IActivityLogRepository
{
    public async Task LogAsync(string? userId, string action, string detail = "")
    {
        const string sql = "INSERT INTO ActivityLogs (UserId, Action, Detail, CreatedAt) VALUES (@userId, @action, @detail, @createdAt);";
        using var conn = factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { userId, action, detail, createdAt = DateTime.UtcNow });
    }

    public async Task<IReadOnlyList<ActivityLog>> RecentAsync(int take = 10)
    {
        const string sql = @"
SELECT a.Id, a.UserId, u.FullName AS UserName, a.Action, a.Detail, a.CreatedAt
FROM ActivityLogs a
LEFT JOIN Users u ON u.Id = a.UserId
ORDER BY a.CreatedAt DESC, a.Id DESC
LIMIT @take;";
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync<ActivityLog>(sql, new { take })).ToList();
    }
}
