using MoreLinq;

namespace AOC.Solutions;

public class Day08 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var forest = ParseForest(lines.ToList());
        var blockedDirections = Traverse(forest, (size, lineSizes) => lineSizes.Any(s => s >= size) ? 1 : 0, (a, b) => a + b);
        var scenicScores = Traverse(forest, (size, lineSizes) => lineSizes.TakeUntil(s => s >= size).Count(), (a, b) => a * b);

        yield return blockedDirections.Where(kv => kv.Value < 4).Count().ToString();
        yield return scenicScores.Values.Max().ToString();
    }

    private Dictionary<Vector, T> Traverse<T>(Forest forest, Func<int, IEnumerable<int>, T> evaluateLine, Func<T, T, T> aggregateResults)
    {
        var trees = forest.Trees;
        var result = new Dictionary<Vector, T>();
        foreach (var position in trees.Keys)
        {
            var cuts = forest.Indexes.SelectMany(p => new[] { new Vector(p, position.Y), new Vector(position.X, p) });
            var sortedCuts = cuts.OrderBy(p => p.ManhattanDistance(position));
            var lines = sortedCuts.Where(p => p != position).GroupBy(p => Direction(p, position));
            var results = lines.Select(d => evaluateLine(trees[position], d.Select(p => trees[p])));

            result[position] = results.Aggregate(aggregateResults);
        }

        return result;
    }

    private Vector Direction(Vector from, Vector to)
    {
        return new Vector(from.X.CompareTo(to.X), from.Y.CompareTo(to.Y));
    }

    private Forest ParseForest(List<string> lines)
    {
        var size = lines.Count();
        var indexes = Enumerable.Range(0, size);

        var trees = new Dictionary<Vector, int>();
        foreach (var y in indexes)
        {
            var line = lines[y].ToCharArray();
            foreach (var x in indexes)
            {
                trees[new Vector(x, y)] = line[x].ToInt();
            }
        }

        return new Forest(size, indexes, trees);
    }

    private record Forest(int Size, IEnumerable<int> Indexes, Dictionary<Vector, int> Trees);
}