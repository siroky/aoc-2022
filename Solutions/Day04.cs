namespace AOC.Solutions;

public class Day04 : ISolver
{
    private static readonly ComparableTotalOrder<int> Intervals = new ComparableTotalOrder<int>();

    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var intervalPairs = lines.Select(l => ParseIntervals(l));
        var containingPairs = intervalPairs.Where(p =>
            Intervals.Contains(p.Item1, p.Item2) ||
            Intervals.Contains(p.Item2, p.Item1)
        );
        var overlappingPairs = intervalPairs.Where(p => Intervals.Intersects(p.Item1, p.Item2));

        yield return containingPairs.Count().ToString();
        yield return overlappingPairs.Count().ToString();
    }

    private (Interval<int>, Interval<int>) ParseIntervals(string line)
    {
        var intervals = line.Split(',');
        return (ParseInterval(intervals[0]), ParseInterval(intervals[1]));
    }

    private Interval<int> ParseInterval(string i)
    {
        var bounds = i.Split('-');
        var lowerBound = bounds[0].ToInt();
        var upperBound = bounds[1].ToInt();
        return Intervals.ClosedInterval(lowerBound, upperBound);
    }
}