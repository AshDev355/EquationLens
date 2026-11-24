using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EquationLens.Models;

/// <summary>Custom Identity user with extra profile fields.</summary>
public class ApplicationUser : IdentityUser
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }
}
