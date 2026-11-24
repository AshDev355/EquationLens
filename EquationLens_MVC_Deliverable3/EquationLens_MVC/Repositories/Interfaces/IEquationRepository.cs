using EquationLens.Models;

namespace EquationLens.Repositories;

public interface IEquationRepository
{
    /// <summary>Inserts the equation and returns its new Id.</summary>
    Task<int> AddAsync(EquationRecord equation);
    Task<IReadOnlyList<EquationRecord>> GetByUserAsync(string userId, int take = 20);
    Task<bool> DeleteAsync(int id, string userId);
    Task<int> DeleteAllByUserAsync(string userId);
    Task<int> CountAsync();
}
