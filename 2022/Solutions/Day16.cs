using System.Text.RegularExpressions;

namespace AOC.Solutions;

public class Day16 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var graph = ParseGraph(lines, "AA");

        var player1 = new Seeker("Player 1", graph.Origin, 30);
        var pressure1 = Solve(graph, graph.Valves, player1.ToEnumerable().ToList());

        var player2 = new Seeker("Player 2", graph.Origin, 26);
        var elephant = new Seeker("Elephant", graph.Origin, 26);
        var pressure2 = Solve(graph, graph.Valves, player2.ToEnumerable().Concat(elephant).ToList());

        yield return pressure1.ToString();
        yield return pressure2.ToString();
    }

    private int Solve(Graph graph, List<Valve> availableValves, List<Seeker> seekers)
    {
        var seeker = seekers.MaxBy(s => s.Time);
        var otherSeekers = seekers.Except(seeker.ToEnumerable()).ToList();

        var allOptions = availableValves.Select(valve =>
        {
            var distance = graph.Distances[(seeker.Valve, valve)];
            var remainingTime = seeker.Time - distance - 1;
            var pressure = valve.Flow * Math.Max(0, remainingTime);
            return (valve, distance, remainingTime, pressure);
        });
        var options = allOptions.Where(o => o.pressure > 0).ToList();

        var results = new List<(Valve, int)>();
        foreach (var option in options)
        {
            var newSeeker = new Seeker(seeker.Name, option.valve, option.remainingTime);
            var newSeekers = otherSeekers.Concat(newSeeker).ToList();

            var otherValves = availableValves.Except(option.valve.ToEnumerable()).ToList();
            var otherPressures = Solve(graph, otherValves, newSeekers);
            var totalPressure = option.pressure + otherPressures;
            results.Add((option.valve, totalPressure));
        }

        return results.IsEmpty() ? 0 : results.Select(r => r.Item2).Max();
    }

    private Dictionary<Valve, int> GetDistances(ILookup<Valve, Valve> edges, Valve start)
    {
        var result = new Dictionary<Valve, int>();
        var next = start.ToEnumerable().ToHashSet();
        var distance = 0;

        while (next.Any())
        {
            foreach (var valve in next)
            {
                result[valve] = distance;
            }
            next = next.SelectMany(n => edges[n]).Except(result.Keys).ToHashSet();
            distance += 1;
        }

        return result;
    }

    private Graph ParseGraph(IEnumerable<string> lines, string originName)
    {
        var allValves = lines.Select(l => ParseValve(l)).ToDictionary(v => v.Name, v => v);
        var origin = allValves[originName];
        var valves = allValves.Values.Where(v => v.Flow > 0).Concat(origin).ToHashSet();

        var edges = allValves.Values.SelectMany(v1 => v1.Tunnels.Select(t => (
            Source: v1,
            Target: allValves[t]
        )));
        var edgeLookup = edges.ToLookup(e => e.Source, e => e.Target);

        var distances = valves.SelectMany(v => GetDistances(edgeLookup, v).Select(kv => (
            Source: v,
            Target: kv.Key,
            Distance: kv.Value
        )));
        var relevantDistances = distances.Where(d => valves.Contains(d.Target));

        return new Graph(
            Valves: valves.ToList(),
            Origin: origin,
            Distances: relevantDistances.ToDictionary(d => (d.Source, d.Target), d => d.Distance)
        );
    }

    private Valve ParseValve(string line)
    {
        var words = Regex.Replace(line, "[,;]+", "").Words();
        var name = words.Second();
        var flow = words.Fifth().Split("=").Second().ToInt();
        var tunnels = words.Reverse().TakeWhile(w => !w.StartsWith("valve"));

        return new Valve(name, flow, tunnels.ToList());
    }

    private record Graph(List<Valve> Valves, Valve Origin, Dictionary<(Valve, Valve), int> Distances);
    private record Valve(string Name, int Flow, IEnumerable<string> Tunnels);
    private record Seeker(string Name, Valve Valve, int Time);
}