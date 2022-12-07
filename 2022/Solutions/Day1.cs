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
        return lines.Partition(l => l.IsBlank()).Select(p => ParsePartition(p));
    }

    private IEnumerable<int> ParsePartition(IEnumerable<string> lines)
    {
        var cleanLines = lines.Where(l => !l.IsBlank());
        return cleanLines.Select(l => l.ToInt());
    }
}
