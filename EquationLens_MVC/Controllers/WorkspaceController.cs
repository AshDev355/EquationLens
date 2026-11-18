using System.Security.Claims;
using System.Text.Json;
using EquationLens.Data;
using EquationLens.Models;
using EquationLens.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EquationLens.Controllers;
[Authorize(Roles="User,Admin")]
public class WorkspaceController(ApplicationDbContext db) : Controller {
 public async Task<IActionResult> Index(){var id=User.FindFirstValue(ClaimTypes.NameIdentifier)!; var user=await db.Users.FindAsync(id); var history=await db.EquationRecords.Where(x=>x.UserId==id).OrderByDescending(x=>x.CreatedAt).Take(20).ToListAsync(); return View(new WorkspaceViewModel{FullName=user?.FullName??User.Identity?.Name??"Student",History=history});}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Solve(string equation){var id=User.FindFirstValue(ClaimTypes.NameIdentifier)!; var user=await db.Users.FindAsync(id); var vm=new WorkspaceViewModel{FullName=user?.FullName??"Student",Equation=equation,History=await db.EquationRecords.Where(x=>x.UserId==id).OrderByDescending(x=>x.CreatedAt).Take(20).ToListAsync()}; try{var result=EquationSolver.Solve(equation); vm.Steps=result.Steps; db.EquationRecords.Add(new EquationRecord{Equation=equation,Result=result.Answer,StepsJson=JsonSerializer.Serialize(result.Steps),UserId=id}); await db.SaveChangesAsync(); vm.History=await db.EquationRecords.Where(x=>x.UserId==id).OrderByDescending(x=>x.CreatedAt).Take(20).ToListAsync();}catch(ArgumentException ex){vm.Error=ex.Message;} return View("Index",vm);}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> ClearHistory(){var id=User.FindFirstValue(ClaimTypes.NameIdentifier)!; var rows=db.EquationRecords.Where(x=>x.UserId==id); db.EquationRecords.RemoveRange(rows); await db.SaveChangesAsync(); return RedirectToAction(nameof(Index));}
}
