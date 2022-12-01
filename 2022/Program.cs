namespace AOC
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Day1();
        }

        private static void Day1()
        {
            var lines = GetInputLines(1);
            var partitions = PartitionByBlanks(lines);
            var sums = partitions.Select(p => p.Select(l => Int32.Parse(l)).Sum());
            var leaderboard = sums.OrderByDescending(s => s).ToList();

            Console.WriteLine($"1.1 - {leaderboard[0]}");
            Console.WriteLine($"1.2 - {leaderboard[0] + leaderboard[1] + leaderboard[2]}");
        }

        private static IEnumerable<IEnumerable<string>> PartitionByBlanks(IEnumerable<string> lines)
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

        private static IEnumerable<string> GetInputLines(int day)
        {
            return File.ReadAllLines($"Inputs/{day}.txt");
        }
    }
}