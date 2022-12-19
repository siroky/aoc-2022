namespace AOC.Solutions;

public class Day18 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var cubes = lines.Select(l => Vector.Parse(l.Split(","))).ToList();
        var totalSurface = Surface(cubes, external: false);
        var externalSurface = Surface(cubes, external: true);

        yield return totalSurface.ToString();
        yield return externalSurface.ToString();
    }

    private int Surface(List<Vector> cubes, bool external)
    {
        var grid = cubes.ToDictionary(c => c, c => false);
        var min = Vector.Zero.Subtract(Vector.Unit);
        var max = Vector.Max(cubes).Add(Vector.Unit);

        var next = min.ToEnumerable();
        while (external && next.Any())
        {
            grid.Add(next, true);

            var adjacent = next.SelectMany(c => c.AdjacentXYZ()).Distinct();
            var boundedAdjacent = adjacent.Where(c => c.LessOrEquals(max) && min.LessOrEquals(c));
            next = boundedAdjacent.Where(c => !grid.ContainsKey(c)).ToList();
        }

        var adjacentCubes = cubes.SelectMany(c => c.AdjacentXYZ());
        return adjacentCubes.Count(c => grid.Get(c).GetOrElse(!external));
    }
}