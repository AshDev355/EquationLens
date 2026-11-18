using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace EquationLens.Models;
public class ApplicationUser : IdentityUser {
    [Required, StringLength(100)] public string FullName { get; set; } = "";
    public ICollection<EquationRecord> Equations { get; set; } = new List<EquationRecord>();
}
