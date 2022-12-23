namespace AOC.Solutions;

public class Day23 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var elves = ParseElves(lines).ToHashSet();
        var result = Simulate(elves, rounds: 10);

        var min = new Vector(result.Min(p => p.X), result.Min(p => p.Y));
        var max = new Vector(result.Max(p => p.X), result.Max(p => p.Y));
        var size = max.Subtract(min).Add(Vector.Unit);
        var count = size.X * size.Y - elves.Count;

        var stableRound = Stabilize(elves);

        yield return count.ToString();
        yield return stableRound.ToString();
    }

    private HashSet<Vector> Simulate(HashSet<Vector> elves, int rounds)
    {
        for (var round = 0; round < rounds; round++)
        {
            elves = SimulateRound(elves, round);
        }

        return elves;
    }

    private int Stabilize(HashSet<Vector> elves)
    {
        var round = 0;
        while (true)
        {
            var newElves = SimulateRound(elves, round++);
            if (elves.Except(newElves).IsEmpty())
            {
                return round;
            }

            elves = newElves;
        }
    }

    private HashSet<Vector> SimulateRound(HashSet<Vector> elves, int round)
    {
        var proposals = elves.ToDictionary(e => e, e =>
        {
            var options = MoveOptions(e, round);
            var considerations = options.Where(positions => !positions.Any(p => elves.Contains(p))).ToList();
            return considerations.Count.Match(
                options.Count, _ => Option.Empty<Vector>(),
                _ => considerations.FirstOption().Map(p => p.Second())
            );
        });
        var destinations = proposals.Values.Flatten();
        var destinationCounts = destinations.GroupBy(p => p).ToDictionary(g => g.Key, g => g.Count());

        var result = new HashSet<Vector>();
        foreach (var (elf, proposal) in proposals)
        {
            result.Add(proposal.Match(
                p => destinationCounts[p].Match(
                    1, _ => p,
                    _ => elf
                ),
                _ => elf
            ));
        }

        return result;
    }

    private List<List<Vector>> MoveOptions(Vector elf, int round)
    {
        var considerations = new[]
        {
            new List<Vector>
            {
                elf.Add(Vector.Up).Add(Vector.Left),
                elf.Add(Vector.Up),
                elf.Add(Vector.Up).Add(Vector.Right)
            },
            new List<Vector>
            {
                elf.Add(Vector.Down).Add(Vector.Left),
                elf.Add(Vector.Down),
                elf.Add(Vector.Down).Add(Vector.Right)
            },
            new List<Vector>
            {
                elf.Add(Vector.Left).Add(Vector.Up),
                elf.Add(Vector.Left),
                elf.Add(Vector.Left).Add(Vector.Down)
            },
            new List<Vector>
            {
                elf.Add(Vector.Right).Add(Vector.Up),
                elf.Add(Vector.Right),
                elf.Add(Vector.Right).Add(Vector.Down)
            }
        };

        return considerations.Skip(round % 4).Concat(considerations.Take(round % 4)).ToList();
    }

    private IEnumerable<Vector> ParseElves(IEnumerable<string> lines)
    {
        for (var y = 0; y < lines.Count(); y++)
        {
            var line = lines.ElementAt(y);
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    yield return new Vector(x, y);
                }
            }
        }
    }
}