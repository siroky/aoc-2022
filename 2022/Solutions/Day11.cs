using MoreLinq;

namespace AOC.Solutions;

public class Day11 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var monkeys = lines.Paragraphs().Select(p => ParseMonkey(p)).ToList();
        var divisorProduct = monkeys.Select(m => m.Behavior.Divisor).Product();

        var results1 = ExecuteRounds(20, monkeys, adjustWorry: w => w / 3);
        var results2 = ExecuteRounds(10_000, monkeys, adjustWorry: w => w % divisorProduct);

        yield return MonkeyBusiness(results1);
        yield return MonkeyBusiness(results2);
    }

    private string MonkeyBusiness(IEnumerable<IEnumerable<Monkey>> results)
    {
        var roundInspections = results.Select(r => r.Select(m => m.Inspections));
        var monkeyInspections = roundInspections.Transpose().Select(i => (long)i.Sum());
        var leaderhoard = monkeyInspections.OrderByDescending(c => c);

        return leaderhoard.Take(2).Product().ToString();
    }

    private IEnumerable<IEnumerable<Monkey>> ExecuteRounds(int count, IEnumerable<Monkey> monkeys, Func<long, long> adjustWorry)
    {
        for (var i = 0; i < count; i++)
        {
            monkeys = ExecuteRound(monkeys, adjustWorry).ToList();
            yield return monkeys;
        }
    }

    private IEnumerable<Monkey> ExecuteRound(IEnumerable<Monkey> monkeys, Func<long, long> adjustWorry)
    {
        var catches = new DataCube1<int, IEnumerable<long>>();
        var inspections = new List<long>();

        for (var i = 0; i < monkeys.Count(); i++)
        {
            var monkey = monkeys.ElementAt(i);
            var caughtItems = catches.Get(i).Flatten();
            catches.Set(i, Enumerable.Empty<long>());

            var items = monkey.Items.Concat(caughtItems);
            inspections.Add(items.Count());

            var worries = items.Select(i => adjustWorry(monkey.Behavior.Worry(i)));
            var throws = worries.ToLookup(i => monkey.Behavior.Throw(i));
            foreach (var target in throws)
            {
                catches.SetOrElseUpdate(target.Key, target, (a, b) => a.Concat(b));
            }
        }

        return monkeys.Select((m, i) => new Monkey(
            Behavior: m.Behavior,
            Items: catches.Get(i).Flatten().ToList(),
            Inspections: inspections.ElementAt(i)
        ));
    }

    private Monkey ParseMonkey(IEnumerable<string> lines)
    {
        var itemWords = lines.Second().Split(':').Second().Replace(",", "").Words();
        var items = itemWords.Select(w => w.ToLong());

        var operationWords = lines.Third().Split('=').Second().Words();
        var function = (long a, long b) => operationWords.Second().Match(
            "+", _ => a + b,
            "*", _ => a * b
        );
        var firstArgument = operationWords.First().ToLongOption();
        var secondArgument = operationWords.Third().ToLongOption();

        var divisor = lines.Fourth().Words().Last().ToLong();
        var success = lines.Fifth().Words().Last().ToInt();
        var failure = lines.Sixth().Words().Last().ToInt();

        return new Monkey(
            Behavior: new MonkeyBehavior(
                Worry: old => function(
                    firstArgument.GetOrElse(old),
                    secondArgument.GetOrElse(old)
                ),
                Divisor: divisor,
                Throw: worry => (worry % divisor == 0).Match(
                    t => success,
                    f => failure
                )
            ),
            Items: items
        );
    }

    public record Monkey(MonkeyBehavior Behavior, IEnumerable<long> Items, long Inspections = 0);
    public record MonkeyBehavior(Func<long, long> Worry, long Divisor, Func<long, int> Throw);
}