using EquationLens.Models;
using EquationLens.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquationLens.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(
    IUserRepository userRepository,
    IEquationRepository equations,
    IActivityLogRepository activity) : Controller
{
    private const int PageSize = 8;

    public async Task<IActionResult> Index()
    {
        var vm = new AdminDashboardViewModel
        {
            UserCount = await userRepository.CountAsync(),
            EquationCount = await equations.CountAsync(),
            Users = await BuildUsersPage(null, 1),
            Activity = await activity.RecentAsync(10)
        };
        return View(vm);
    }

    // ---------- AJAX endpoints (return partial views) ----------

    [HttpGet]
    public async Task<IActionResult> UsersTable(string? term, int page = 1)
        => PartialView("_UsersTable", await BuildUsersPage(term, page));

    [HttpGet]
    public async Task<IActionResult> ActivityFeed()
        => PartialView("_Activity", await activity.RecentAsync(10));

    private async Task<UsersPageViewModel> BuildUsersPage(string? term, int page)
    {
        page = Math.Max(page, 1);
        var (items, total) = await userRepository.SearchAsync(term, page, PageSize);
        return new UsersPageViewModel { Items = items, Page = page, PageSize = PageSize, Total = total, Term = term?.Trim() ?? "" };
    }
}
