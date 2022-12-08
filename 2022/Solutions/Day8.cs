namespace AOC.Solutions;

public class Day8 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var forest = ParseForest(lines.ToList());
        var visibility = Traverse(forest, (size, lineSizes) => lineSizes.Any(s => s >= size) ? 1 : 0, (a, b) => a + b);
        var scores = Traverse(forest, (size, lineSizes) => lineSizes.TakeUntil(s => s >= size).Count(), (a, b) => a * b);

        yield return visibility.Where(kv => kv.Value < 4).Count().ToString();
        yield return scores.Values.Max().ToString();
    }

    private Dictionary<(int X, int Y), T> Traverse<T>(Forest forest, Func<int, IEnumerable<int>, T> lineResult, Func<T, T, T> aggregateResults)
    {
        var trees = forest.Trees;
        var result = new Dictionary<(int, int), T>();
        foreach (var position in trees.Keys)
        {
            var crossPositions = forest.Indexes.SelectMany(p => new[] { (X: p, Y: position.Y), (X: position.X, Y: p) });
            var otherPositions = crossPositions.Where(p => p != position);
            var neighbors = otherPositions.OrderBy(p => Math.Abs(position.X - p.X) + Math.Abs(position.Y - p.Y));

            var lines = neighbors.GroupBy(p => (p.X.CompareTo(position.X), p.Y.CompareTo(position.Y)));
            var results = lines.Select(l => lineResult(trees[position], l.Select(p => trees[p])));

            result[position] = results.Aggregate(aggregateResults);
        }

        return result;
    }

    private Forest ParseForest(List<string> lines)
    {
        var size = lines.Count();
        var indexes = Enumerable.Range(0, size);

        var trees = new Dictionary<(int, int), int>();
        foreach (var y in indexes)
        {
            var line = lines[y].ToCharArray();
            foreach (var x in indexes)
            {
                trees[(x, y)] = line[x].ToInt();
            }
        }

        return new Forest(size, indexes, trees);
    }


    public record Forest(int Size, IEnumerable<int> Indexes, Dictionary<(int X, int Y), int> Trees);
}