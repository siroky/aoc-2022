namespace AOC.Solutions;

public class Day1 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var partitions = ParseInput(lines);
        var sums = partitions.Select(p => p.Sum());
        var leaderboard = sums.OrderByDescending(s => s).ToList();

        yield return leaderboard[0].ToString();
        yield return (leaderboard[0] + leaderboard[1] + leaderboard[2]).ToString();
    }

    private IEnumerable<IEnumerable<int>> ParseInput(IEnumerable<string> lines)
    {
        var partition = new List<int>();
        foreach (var line in lines)
        {
            if (String.IsNullOrEmpty(line))
            {
                if (partition.Any())
                {
                    yield return partition;
                    partition = new List<int>();
                }
            }
            else
            {
                partition.Add(Int32.Parse(line));
            }
        }

        if (partition.Any())
        {
            yield return partition;
        }
    }
}
