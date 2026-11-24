namespace EquationLens.Services;

public interface IEquationSolverService
{
    /// <exception cref="ArgumentException">The equation is empty or not a supported linear/quadratic equation.</exception>
    SolveResult Solve(string equation);
}

public class EquationSolverService : IEquationSolverService
{
    public SolveResult Solve(string equation) => EquationSolver.Solve(equation);
}
