using EquationLens.Models;

namespace EquationLens.Repositories;

public interface IActivityLogRepository
{
    Task LogAsync(string? userId, string action, string detail = "");
    Task<IReadOnlyList<ActivityLog>> RecentAsync(int take = 10);
}
