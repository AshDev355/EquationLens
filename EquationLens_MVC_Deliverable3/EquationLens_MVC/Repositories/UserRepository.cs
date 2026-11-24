using Dapper;
using EquationLens.Data;
using EquationLens.Models;

namespace EquationLens.Repositories;

public class UserRepository(IDbConnectionFactory factory) : IUserRepository
{
    public async Task<(IReadOnlyList<UserSummary> Items, int Total)> SearchAsync(string? term, int page, int pageSize)
    {
        var like = $"%{(term ?? "").Trim()}%";
        var offset = (Math.Max(page, 1) - 1) * pageSize;

        const string listSql = @"
SELECT u.Id,
       u.FullName AS Name,
       u.Email,
       CASE WHEN EXISTS (SELECT 1 FROM UserRoles ur JOIN Roles r ON r.Id = ur.RoleId
                         WHERE ur.UserId = u.Id AND r.Name = 'Admin')
            THEN 'Admin' ELSE 'User' END AS Role,
       (SELECT COUNT(*) FROM Equations e WHERE e.UserId = u.Id) AS Solved,
       u.CreatedAt
FROM Users u
WHERE u.FullName LIKE @like OR u.Email LIKE @like
ORDER BY u.FullName
LIMIT @pageSize OFFSET @offset;";

        const string countSql = "SELECT COUNT(*) FROM Users u WHERE u.FullName LIKE @like OR u.Email LIKE @like;";

        using var conn = factory.CreateConnection();
        var items = (await conn.QueryAsync<UserSummary>(listSql, new { like, pageSize, offset })).ToList();
        var total = await conn.ExecuteScalarAsync<int>(countSql, new { like });
        return (items, total);
    }

    public async Task<int> CountAsync()
    {
        using var conn = factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users;");
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var conn = factory.CreateConnection();
        var count = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Users WHERE NormalizedEmail = @normalized;",
            new { normalized = email.Trim().ToUpperInvariant() });
        return count > 0;
    }
}
