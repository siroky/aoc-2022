namespace AOC.Solutions;

public class Day25 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var numbers = lines.Select(l => SnafuToLong(l)).ToList();
        var sum = numbers.Sum();
        var sumSnafu = LongToSnafu(sum);

        yield return sumSnafu;
    }

    private string LongToSnafu(long number)
    {
        var digits = new List<string>();
        while (number > 0)
        {
            var remainder = number % 5;
            number /= 5;

            digits.Add(remainder.Match(
                0, _ => "0",
                1, _ => "1",
                2, _ => "2",
                3, _ => "=",
                4, _ => "-"
            ));
            if (remainder > 2)
            {
                number += 1;
            }
        }

        return String.Concat(digits.AsEnumerable().Reverse());
    }

    private long SnafuToLong(string snafu)
    {
        var digits = snafu.ToCharArray().Reverse();
        var result = 0L;
        var multiplier = 1L;
        foreach (var digit in digits)
        {
            result += multiplier * digit.Match(
                '2', _ => 2,
                '1', _ => 1,
                '0', _ => 0,
                '-', _ => -1,
                '=', _ => -2
            );

            multiplier *= 5;
        }

        return result;
    }
}