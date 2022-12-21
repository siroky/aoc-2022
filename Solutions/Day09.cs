namespace AOC.Solutions;

public class Day09 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var steps = ParseSteps(lines).ToList();

        yield return Solve(steps, 2);
        yield return Solve(steps, 10);
    }

    private string Solve(IEnumerable<Step> steps, int length)
    {
        var initial = new Rope(length.Generate(_ => Vector.Zero).ToList());
        var ropes = Execute(steps, initial);

        return ropes.Select(r => r.Knots.Last()).Distinct().Count().ToString();
    }

    private IEnumerable<Rope> Execute(IEnumerable<Step> steps, Rope initial)
    {
        var rope = initial;
        yield return rope;

        foreach (var step in steps)
        {
            for (var i = 0; i < step.Count; i++)
            {
                rope = ExecuteStep(rope, step.Direction);
                yield return rope;
            }
        }
    }

    private Rope ExecuteStep(Rope rope, Vector direction)
    {
        var knots = rope.Knots;
        var newHead = knots.First().Add(direction);
        var newKnots = newHead.ToEnumerable().ToList();

        foreach (var knot in knots.Skip(1))
        {
            var previous = newKnots.Last();
            var diff = previous.Subtract(knot);
            var move = (diff.Length() < 2).Match(
                t => Vector.Zero,
                f => diff.Sign()
            );

            newKnots.Add(knot.Add(move));
        }

        return new Rope(newKnots);
    }

    private IEnumerable<Step> ParseSteps(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var words = line.Words();
            yield return new Step(
                Direction: words.First().Match(
                    "U", _ => new Vector(0, 1),
                    "D", _ => new Vector(0, -1),
                    "L", _ => new Vector(-1, 0),
                    "R", _ => new Vector(1, 0)
                ),
                Count: words.Second().ToInt()
            );
        }
    }

    private record Rope(List<Vector> Knots);
    private record Step(Vector Direction, int Count);
}