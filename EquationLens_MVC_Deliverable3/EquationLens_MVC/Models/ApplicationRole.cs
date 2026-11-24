using Microsoft.AspNetCore.Identity;

namespace EquationLens.Models;

/// <summary>Custom Identity role with a description.</summary>
public class ApplicationRole : IdentityRole
{
    public ApplicationRole() { }
    public ApplicationRole(string name, string description) : base(name) => Description = description;

    public string Description { get; set; } = "";
}
