using EquationLens.Models;
using Microsoft.AspNetCore.Identity;

namespace EquationLens.Identity;

/// <summary>Rejects passwords that contain the user's name or the first part of their email.</summary>
public class NoPersonalInfoPasswordValidator : IPasswordValidator<ApplicationUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string? password)
    {
        if (string.IsNullOrEmpty(password)) return Task.FromResult(IdentityResult.Success);

        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(user.Email)) parts.Add(user.Email.Split('@')[0]);
        if (!string.IsNullOrWhiteSpace(user.FullName))
            parts.AddRange(user.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        foreach (var part in parts)
        {
            if (part.Length >= 3 && password.Contains(part, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "PasswordContainsPersonalInfo",
                    Description = "Your password must not contain your name or the first part of your email."
                }));
            }
        }
        return Task.FromResult(IdentityResult.Success);
    }
}
