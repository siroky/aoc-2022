namespace AOC.Solutions;

public class Day13 : Comparer<Day13.Packet>, ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var pairs = lines.Paragraphs().Select(p => p.Select(l => ParsePacket(l)).ToList()).ToList();
        var orderedPairs = pairs.Select((pair, i) => (i + 1, Less(pair.First(), pair.Second()).Get()));

        var dividers = new[] { new Packet(new Packet(new Packet(2))), new Packet(new Packet(new Packet(6))) };
        var packets = pairs.Flatten().Concat(dividers).ToList();
        var orderedPackets = packets.OrderBy(p => p, this).ToList();
        var dividerIndexes = orderedPackets.Select((p, i) => (i + 1, dividers.Contains(p)));

        yield return orderedPairs.Where(c => c.Item2).Sum(c => c.Item1).ToString();
        yield return dividerIndexes.Where(i => i.Item2).Select(i => i.Item1).Product().ToString();
    }

    public override int Compare(Packet? x, Packet? y)
    {
        return Less(x, y).Match(
            l => l.Match(
                t => -1,
                f => 1
            ),
            _ => 0
        );
    }

    private IOption<bool> Less(Packet left, Packet right)
    {
        return left.Match(
            a => right.Match(
                 b => (a != b).Match(
                    t => (a < b).ToOption(),
                    f => Option.Empty<bool>()
                 ),
                 q => Less(new Packet(left), right)
            ),
            p => right.Match(
                 b => Less(left, new Packet(right)),
                 q => Less(p, q)
            )
        );
    }

    private IOption<bool> Less(IEnumerable<Packet> left, IEnumerable<Packet> right)
    {
        if (!left.Any() && !right.Any())
        {
            return Option.Empty<bool>();
        }
        if (!left.Any())
        {
            return true.ToOption();
        }
        if (!right.Any())
        {
            return false.ToOption();
        }

        var headsOrdered = Less(left.First(), right.First());
        var tailsOrdered = Less(left.Skip(1), right.Skip(1));
        return headsOrdered.OrElse(_ => tailsOrdered);
    }

    private Packet ParsePacket(string text)
    {
        return text.StartsWith('[').Match(
            t => new Packet(ParseItems(text.Substring(1, text.Length - 2))),
            f => new Packet(text.ToInt())
        );
    }

    private IEnumerable<Packet> ParseItems(string text)
    {
        if (text.IsBlank())
        {
            return Enumerable.Empty<Packet>();
        }

        var chars = text.ToCharArray();
        var indexes = Enumerable.Range(0, chars.Count()).Where(i => chars[i] == ',');
        var splitIndex = indexes.FirstOption(i => chars.Take(i).Count('[') == chars.Take(i).Count(']'));

        var firstItemLength = splitIndex.GetOrElse(text.Length);
        var firstItem = ParsePacket(text.Substring(0, firstItemLength));
        var otherItems = splitIndex.Map(i => ParseItems(text.Substring(i + 1)));

        return firstItem.ToEnumerable().Concat(otherItems.Flatten()).ToList();
    }

    public class Packet : Coproduct2<int, IEnumerable<Packet>>
    {
        public Packet(int i) : base(i) { }
        public Packet(IEnumerable<Packet> p) : base(p) { }
        public Packet(Packet p) : this(p.ToEnumerable()) { }
    }
}