namespace AOC.Solutions;

public class Day24 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var map = ParseMap(lines);
        var steps = Escape(map, map.Start, map.End);
        var steps2 = Escape(map, map.End, map.Start, steps);
        var steps3 = Escape(map, map.Start, map.End, steps2);

        yield return steps.ToString();
        yield return steps3.ToString();
    }

    private int Escape(Map map, Vector start, Vector end, int step = 0)
    {
        var positions = start.ToEnumerable().ToHashSet();

        while (true)
        {
            if (positions.Contains(end))
            {
                return step;
            }

            var options = positions.SelectMany(p => p.WithAdjacentXY()).Where(p => !map.Walls.Contains(p));
            var storms = map.Storms.Select(s => StormPosition(map, s, step + 1));

            positions = options.Except(storms).ToHashSet();
            step += 1;
        }
    }

    private Vector StormPosition(Map map, Storm storm, int step)
    {
        var position = storm.Start.Add(storm.Direction.Multiply(step));
        return new Vector(
            X: Mod(position.X - 1, map.Size.X) + 1,
            Y: Mod(position.Y - 1, map.Size.Y) + 1
        );
    }

    private long Mod(long a, long b)
    {
        return (a % b + b) % b;
    }

    private Map ParseMap(IEnumerable<string> lines)
    {
        var walls = new HashSet<Vector> { new Vector(1, -1) };
        var storms = new List<Storm>();

        for (var y = 0; y < lines.Count(); y++)
        {
            var line = lines.ElementAt(y);
            for (var x = 0; x < line.Length; x++)
            {
                var position = new Vector(x, y);
                line[x].Match(
                    '#', _ => walls.Add(position),
                    '^', _ => storms.Add(new Storm(position, Vector.Up)),
                    'v', _ => storms.Add(new Storm(position, Vector.Down)),
                    '<', _ => storms.Add(new Storm(position, Vector.Left)),
                    '>', _ => storms.Add(new Storm(position, Vector.Right)),
                    '.', _ => { }
                );
            }
        }

        var max = new Vector(walls.Max(p => p.X), walls.Max(p => p.Y));

        return new Map(
            Walls: walls,
            Storms: storms,
            Size: max.AddX(-1).AddY(-1),
            Start: new Vector(1, 0),
            End: max.Add(Vector.Left)
        );
    }

    private record Storm(Vector Start, Vector Direction);
    private record Map(HashSet<Vector> Walls, List<Storm> Storms, Vector Size, Vector Start, Vector End);
}