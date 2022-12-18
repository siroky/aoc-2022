namespace AOC.Solutions;

public class Day18 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var cubes = lines.Select(l => ParseCube(l)).ToList();
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

            var adjacent = next.SelectMany(c => Adjacent(c)).Distinct();
            var boundedAdjacent = adjacent.Where(c => c.PreceedsOrEquals(max) && min.PreceedsOrEquals(c));
            next = boundedAdjacent.Where(c => !grid.ContainsKey(c)).ToList();
        }

        var adjacentCubes = cubes.SelectMany(c => Adjacent(c));
        return adjacentCubes.Count(c => grid.Get(c).GetOrElse(!external));
    }

    private IEnumerable<Vector> Adjacent(Vector cube)
    {
        yield return cube.AddX(1);
        yield return cube.AddX(-1);
        yield return cube.AddY(1);
        yield return cube.AddY(-1);
        yield return cube.AddZ(1);
        yield return cube.AddZ(-1);
    }

    private Vector ParseCube(string line)
    {
        var parts = line.Split(',');
        return new Vector(
            X: parts.First().ToInt(),
            Y: parts.Second().ToInt(),
            Z: parts.Third().ToInt()
        );
    }
}