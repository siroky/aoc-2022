namespace AOC.Solutions
{
    public class Day1 : ISolver
    {
        public IEnumerable<int> Solve(IEnumerable<string> input)
        {
            var partitions = PartitionByBlanks(input);
            var sums = partitions.Select(p => p.Select(l => Int32.Parse(l)).Sum());
            var leaderboard = sums.OrderByDescending(s => s).ToList();

            yield return leaderboard[0];
            yield return leaderboard[0] + leaderboard[1] + leaderboard[2];
        }

        private IEnumerable<IEnumerable<string>> PartitionByBlanks(IEnumerable<string> lines)
        {
            var partition = new List<string>();
            foreach (var line in lines)
            {
                if (String.IsNullOrEmpty(line))
                {
                    if (partition.Any())
                    {
                        yield return partition;
                        partition = new List<string>();
                    }
                }
                else
                {
                    partition.Add(line);
                }
            }

            if (partition.Any())
            {
                yield return partition;
            }
        }
    }
}
