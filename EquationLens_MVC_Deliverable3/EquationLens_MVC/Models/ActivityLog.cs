namespace EquationLens.Models;

/// <summary>Row of the ActivityLogs table, joined with the user's name for display.</summary>
public class ActivityLog
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = "";
    public string Detail { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
