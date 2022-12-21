namespace AOC.Solutions;

public class Day21 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var expressions = lines.Select(l => ParseExpression(l)).ToDictionary(e => e.Name, e => e);
        var result = Evaluate(expressions, "root");

        var root = expressions["root"];
        var rootOperation = root.Second.Get();
        var newRoot = new Expression(new Operation("root", rootOperation.A, rootOperation.B, "-"));
        var newExpressions = expressions.Values.Except(root.ToEnumerable()).Concat(newRoot).ToDictionary(e => e.Name, e => e);
        var humn = Infer(newExpressions, "root", "humn", result: 0);

        yield return result.Get().ToString();
        yield return humn.ToString();
    }

    private IOption<long> Evaluate(Dictionary<string, Expression> expressions, string name, string unknown = null)
    {
        if (name == unknown)
        {
            return Option.Empty<long>();
        }

        return expressions[name].Match(
            c => c.Value.ToOption(),
            o => Evaluate(expressions, o.A, unknown).FlatMap(a =>
                Evaluate(expressions, o.B, unknown).Map(b => o.Operator.Match(
                    "+", _ => a + b,
                    "-", _ => a - b,
                    "*", _ => a * b,
                    "/", _ => a / b
                ))
            )
        );
    }

    private long Infer(Dictionary<string, Expression> expressions, string name, string unknown, long result)
    {
        if (name == unknown)
        {
            return result;
        }

        var operation = expressions[name].Second.Get();
        var a = Evaluate(expressions, operation.A, unknown);
        var b = Evaluate(expressions, operation.B, unknown);

        return a.NonEmpty.Match(
            t => Infer(expressions, operation.B, unknown, operation.Operator.Match(
                "+", _ => result - a.Get(),
                "-", _ => a.Get() - result,
                "*", _ => result / a.Get(),
                "/", _ => a.Get() / result
            )),
            f => Infer(expressions, operation.A, unknown, operation.Operator.Match(
                "+", _ => result - b.Get(),
                "-", _ => result + b.Get(),
                "*", _ => result / b.Get(),
                "/", _ => result * b.Get()
            ))
        );
    }

    private Expression ParseExpression(string line)
    {
        var parts = line.Split(":");
        var name = parts.First();
        var words = parts.Second().Words();

        return words.IsSingle().Match(
            t => new Expression(new Constant(name, words.First().ToLong())),
            f => new Expression(new Operation(name, words.First(), words.Third(), words.Second()))
        );
    }

    private record Constant(string Name, long Value);
    private record Operation(string Name, string A, string B, string Operator);
    private class Expression : Coproduct2<Constant, Operation>
    {
        public Expression(Constant c) : base(c) { }
        public Expression(Operation o) : base(o) { }

        public string Name => Match(c => c.Name, o => o.Name);
    }
}