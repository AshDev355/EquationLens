namespace EquationLens.Models;

/// <summary>Row of the Equations table (read and written with Dapper).</summary>
public class EquationRecord
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Equation { get; set; } = "";
    public string Result { get; set; } = "";
    /// <summary>"typed" or "image".</summary>
    public string Source { get; set; } = "typed";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
