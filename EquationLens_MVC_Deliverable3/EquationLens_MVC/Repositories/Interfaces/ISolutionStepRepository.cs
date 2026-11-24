namespace EquationLens.Repositories;

public interface ISolutionStepRepository
{
    Task AddRangeAsync(int equationId, IEnumerable<string> steps);
    /// <summary>Steps of one equation, only if it belongs to the given user.</summary>
    Task<IReadOnlyList<string>> GetByEquationAsync(int equationId, string userId);
}
