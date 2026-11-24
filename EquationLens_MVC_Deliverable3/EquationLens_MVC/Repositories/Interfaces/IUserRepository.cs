using EquationLens.Models;

namespace EquationLens.Repositories;

/// <summary>Read-only queries over the Identity tables, for the admin screens. Creating users stays with UserManager.</summary>
public interface IUserRepository
{
    Task<(IReadOnlyList<UserSummary> Items, int Total)> SearchAsync(string? term, int page, int pageSize);
    Task<int> CountAsync();
    Task<bool> EmailExistsAsync(string email);
}
