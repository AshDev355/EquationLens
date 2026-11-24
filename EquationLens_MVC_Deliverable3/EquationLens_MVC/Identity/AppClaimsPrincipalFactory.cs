using System.Security.Claims;
using EquationLens.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EquationLens.Identity;

/// <summary>Adds the user's full name to the login cookie so views can greet them without a database call.</summary>
public class AppClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(userManager, roleManager, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("FullName", user.FullName));
        return identity;
    }
}
