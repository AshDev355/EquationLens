using System.Security.Claims;
using EquationLens.Models;
using EquationLens.Repositories;
using EquationLens.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquationLens.Controllers;

[Authorize(Roles = "User,Admin")]
public class WorkspaceController(
    IEquationRepository equations,
    ISolutionStepRepository stepRepository,
    IActivityLogRepository activity,
    IEquationSolverService solver,
    IRecognitionClient recognizer) : Controller
{
    private const long MaxUploadBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedTypes = { "image/png", "image/jpeg" };

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        var vm = new WorkspaceViewModel
        {
            FullName = User.FindFirstValue("FullName") ?? User.Identity?.Name ?? "there",
            History = await equations.GetByUserAsync(UserId)
        };
        return View(vm);
    }

    // ---------- AJAX endpoints ----------

    /// <summary>Returns just the history list (a partial view) so the page can refresh it in place.</summary>
    [HttpGet]
    public async Task<IActionResult> History() => PartialView("_History", await equations.GetByUserAsync(UserId));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Recognize(IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) return BadRequest(new { ok = false, error = "Choose an image first." });
        if (file.Length > MaxUploadBytes) return BadRequest(new { ok = false, error = "The image must be 5 MB or smaller." });
        if (!AllowedTypes.Contains(file.ContentType)) return BadRequest(new { ok = false, error = "Only PNG or JPG images are supported." });

        await using var stream = file.OpenReadStream();
        var result = await recognizer.RecognizeAsync(stream, file.FileName, ct);
        return Json(new { ok = true, latex = result.Latex, isDemo = result.IsDemo, note = result.Note });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Solve([FromForm] string? equation, [FromForm] string? source)
    {
        equation = equation?.Trim() ?? "";
        if (equation.Length > 300) return BadRequest(new { ok = false, error = "That equation is too long." });

        try
        {
            var result = solver.Solve(equation);
            var id = await equations.AddAsync(new EquationRecord
            {
                UserId = UserId,
                Equation = equation,
                Result = result.Answer,
                Source = source == "image" ? "image" : "typed"
            });
            await stepRepository.AddRangeAsync(id, result.Steps);
            await activity.LogAsync(UserId, "Solved an equation", equation);
            return Json(new { ok = true, id, steps = result.Steps, answer = result.Answer });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ok = false, error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Steps(int id) => Json(await stepRepository.GetByEquationAsync(id, UserId));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await equations.DeleteAsync(id, UserId);
        return deleted ? Json(new { ok = true }) : NotFound(new { ok = false, error = "Equation not found." });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearHistory()
    {
        await equations.DeleteAllByUserAsync(UserId);
        await activity.LogAsync(UserId, "Cleared history");
        return Json(new { ok = true });
    }
}
