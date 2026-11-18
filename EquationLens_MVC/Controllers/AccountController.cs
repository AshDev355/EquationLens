using EquationLens.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace EquationLens.Controllers;
public class AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : Controller {
 [HttpGet] public IActionResult Register()=>View(new RegisterViewModel());
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Register(RegisterViewModel model) { if(!ModelState.IsValid)return View(model); var user=new ApplicationUser{UserName=model.Email,Email=model.Email,FullName=model.FullName}; var result=await userManager.CreateAsync(user,model.Password); if(result.Succeeded){await userManager.AddToRoleAsync(user,"User"); await signInManager.SignInAsync(user,false); return RedirectToAction("Index","Workspace");} foreach(var e in result.Errors) ModelState.AddModelError(string.Empty,e.Description); return View(model); }
 [HttpGet] public IActionResult Login(string? returnUrl=null){ViewData["ReturnUrl"]=returnUrl;return View(new LoginViewModel());}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginViewModel model,string? returnUrl=null) { ViewData["ReturnUrl"]=returnUrl; if(!ModelState.IsValid)return View(model); var result=await signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,lockoutOnFailure:true); if(result.Succeeded){var user=await userManager.FindByEmailAsync(model.Email); if(user!=null&&await userManager.IsInRoleAsync(user,"Admin"))return RedirectToAction("Index","Admin"); return RedirectToLocal(returnUrl);} if(result.IsLockedOut)ModelState.AddModelError(string.Empty,"Account temporarily locked. Try again later."); else ModelState.AddModelError(string.Empty,"Invalid email or password."); return View(model); }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Logout(){await signInManager.SignOutAsync();return RedirectToAction("Index","Home");}
 public IActionResult AccessDenied()=>View();
 IActionResult RedirectToLocal(string? url)=>!string.IsNullOrEmpty(url)&&Url.IsLocalUrl(url)?Redirect(url):RedirectToAction("Index","Workspace");
}
