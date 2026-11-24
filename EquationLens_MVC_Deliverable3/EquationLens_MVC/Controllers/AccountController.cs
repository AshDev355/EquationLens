using EquationLens.Models;
using EquationLens.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EquationLens.Controllers;

public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IUserRepository userRepository,
    IActivityLogRepository activity) : Controller
{
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName.Trim() };
        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
            await activity.LogAsync(user.Id, "Signed up");
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Workspace");
        }

        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                await userManager.UpdateAsync(user);
                await activity.LogAsync(user.Id, "Logged in");
                if (await userManager.IsInRoleAsync(user, "Admin")) return RedirectToAction("Index", "Admin");
            }
            return RedirectToLocal(returnUrl);
        }

        ModelState.AddModelError(string.Empty, result.IsLockedOut
            ? "Account temporarily locked. Try again later."
            : "Invalid email or password.");
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();

    /// <summary>AJAX: is this email still free? Used by the sign-up form.</summary>
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> EmailAvailable(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) return Json(new { available = false, message = "Enter a valid email." });
        var exists = await userRepository.EmailExistsAsync(email);
        return Json(new { available = !exists, message = exists ? "This email is already registered." : "This email is available." });
    }

    private IActionResult RedirectToLocal(string? url) =>
        !string.IsNullOrEmpty(url) && Url.IsLocalUrl(url) ? Redirect(url) : RedirectToAction("Index", "Workspace");
}
