namespace EquationLens.Models;

public class WorkspaceViewModel
{
    public string FullName { get; set; } = "";
    public IReadOnlyList<EquationRecord> History { get; set; } = Array.Empty<EquationRecord>();
}

public class UserSummary
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string Role { get; set; } = "User";
    public int Solved { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UsersPageViewModel
{
    public IReadOnlyList<UserSummary> Items { get; set; } = Array.Empty<UserSummary>();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public int Total { get; set; }
    public string Term { get; set; } = "";
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(Total / (double)PageSize));
}

public class AdminDashboardViewModel
{
    public int UserCount { get; set; }
    public int EquationCount { get; set; }
    public UsersPageViewModel Users { get; set; } = new();
    public IReadOnlyList<ActivityLog> Activity { get; set; } = Array.Empty<ActivityLog>();
}
