namespace AOC.Solutions;

public class Day6 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var line = lines.First().ToCharArray();
        var startIndex = IndexOfDistinct(line, 4);
        var messageIndex = IndexOfDistinct(line, 14);

        yield return (startIndex + 4).ToString();
        yield return (messageIndex + 14).ToString();
    }

    private int? IndexOfDistinct(char[] line, int count)
    {
        for (var i = 0; i <= line.Length - count; i++)
        {
            var chars = line.Skip(i).Take(count);
            if (chars.Distinct().Count() == count)
            {
                return i;
            }
        }

        return null;
    }
}