namespace AOC.Solutions;

public interface ISolver
{
    IEnumerable<int> Solve(IEnumerable<string> lines);
}