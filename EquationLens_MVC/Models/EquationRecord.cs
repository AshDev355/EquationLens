using System.ComponentModel.DataAnnotations;
namespace EquationLens.Models;
public class EquationRecord {
    public int Id { get; set; }
    [Required, StringLength(300)] public string Equation { get; set; } = "";
    [Required] public string Result { get; set; } = "";
    public string StepsJson { get; set; } = "[]";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = "";
    public ApplicationUser? User { get; set; }
}
