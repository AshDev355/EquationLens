using EquationLens.Data;
using EquationLens.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EquationLens.Controllers;
[Authorize(Roles="Admin")]
public class AdminController(ApplicationDbContext db,UserManager<ApplicationUser> users) : Controller {
 public async Task<IActionResult> Index(){var all=await users.Users.OrderBy(x=>x.FullName).ToListAsync(); var list=new List<UserSummary>(); foreach(var u in all) list.Add(new UserSummary{Name=u.FullName,Email=u.Email??"",Role=(await users.IsInRoleAsync(u,"Admin"))?"Admin":"User",Solved=await db.EquationRecords.CountAsync(x=>x.UserId==u.Id)}); return View(new AdminDashboardViewModel{UserCount=all.Count,EquationCount=await db.EquationRecords.CountAsync(),Users=list});}
}
