using Dapper;
using EquationLens.Data;

namespace EquationLens.Repositories;

public class SolutionStepRepository(IDbConnectionFactory factory) : ISolutionStepRepository
{
    public async Task AddRangeAsync(int equationId, IEnumerable<string> steps)
    {
        var rows = steps.Select((text, i) => new { EquationId = equationId, StepNumber = i + 1, Text = text }).ToList();
        if (rows.Count == 0) return;

        using var conn = factory.CreateConnection();
        await conn.OpenAsync();
        using var tx = await conn.BeginTransactionAsync();
        await conn.ExecuteAsync(
            "INSERT INTO SolutionSteps (EquationId, StepNumber, Text) VALUES (@EquationId, @StepNumber, @Text);",
            rows, tx);
        await tx.CommitAsync();
    }

    public async Task<IReadOnlyList<string>> GetByEquationAsync(int equationId, string userId)
    {
        const string sql = @"
SELECT s.Text
FROM SolutionSteps s
JOIN Equations e ON e.Id = s.EquationId
WHERE s.EquationId = @equationId AND e.UserId = @userId
ORDER BY s.StepNumber;";
        using var conn = factory.CreateConnection();
        return (await conn.QueryAsync<string>(sql, new { equationId, userId })).ToList();
    }
}
