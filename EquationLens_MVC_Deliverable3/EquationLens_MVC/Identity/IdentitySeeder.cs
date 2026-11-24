using EquationLens.Models;
using Microsoft.AspNetCore.Identity;

namespace EquationLens.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var roles = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var seedRoles = new[]
        {
            new ApplicationRole("User", "Students who upload and solve equations."),
            new ApplicationRole("Admin", "Staff who manage accounts and view activity.")
        };
        foreach (var role in seedRoles)
            if (!await roles.RoleExistsAsync(role.Name!)) await roles.CreateAsync(role);

        // Optional first-run admin. Set AdminSeed:Email and AdminSeed:Password
        // (user-secrets locally, or environment variables AdminSeed__Email / AdminSeed__Password on IIS).
        var email = config["AdminSeed:Email"];
        var password = config["AdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        if (await users.FindByEmailAsync(email) != null) return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = "EquationLens Administrator",
            EmailConfirmed = true
        };
        var result = await users.CreateAsync(admin, password);
        if (result.Succeeded) await users.AddToRoleAsync(admin, "Admin");
    }
}
