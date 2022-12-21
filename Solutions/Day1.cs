namespace AOC.Solutions;

public class Day1 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var partitions = ParsePartitions(lines);
        var sums = partitions.Select(p => p.Sum());
        var leaderboard = sums.OrderByDescending(s => s).ToList();

        yield return leaderboard.First().ToString();
        yield return leaderboard.Take(3).Sum().ToString();
    }

    private IEnumerable<IEnumerable<int>> ParsePartitions(IEnumerable<string> lines)
    {
        return lines.Paragraphs().Select(p => p.Select(l => l.ToInt()));
    }
}
