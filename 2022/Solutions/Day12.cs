namespace AOC.Solutions;

public class Day12 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var graph = ParseGraph(lines);
        var start = graph.Nodes.First(n => n.Character == 'S');
        var end = graph.Nodes.First(n => n.Character == 'E');
        var candidateStarts = graph.Nodes.Where(n => n.Character == 'a');

        yield return GetDistance(graph, start.ToEnumerable(), end).Get().ToString();
        yield return GetDistance(graph, candidateStarts, end).Get().ToString();
    }

    private IOption<int> GetDistance(Graph graph, IEnumerable<Node> starts, Node end)
    {
        var visited = new HashSet<Node>();
        var next = starts.ToHashSet();
        var distance = 0;

        while (next.Any())
        {
            if (next.Contains(end))
            {
                return distance.ToOption();
            }

            visited.UnionWith(next);
            next = next.SelectMany(n => graph.Edges[n]).Except(visited).ToHashSet();
            distance += 1;
        }

        return Option.Empty<int>();
    }

    private Graph ParseGraph(IEnumerable<string> lines)
    {
        var nodes = ParseNodes(lines).ToList();
        var grid = nodes.ToDataCube(n => n.Position, n => n);
        var edges = nodes.SelectMany(node =>
        {
            var directions = Vector.Basis.Concat(Vector.Basis.Select(v => v.Invert()));
            var neighbors = directions.Select(d => grid.Get(node.Position.Add(d))).Flatten();
            var reachable = neighbors.Where(n => n.Height - node.Height <= 1);
            return reachable.Select(n => (node, n));
        });

        return new Graph(nodes, edges.ToLookup(e => e.Item1, e => e.Item2));
    }

    private IEnumerable<Node> ParseNodes(IEnumerable<string> lines)
    {
        return lines.SelectMany((l, y) => l.ToCharArray().Select((c, x) => new Node(
            Character: c,
            Position: new Vector(x, y),
            Height: c.Match(
                'S', _ => 0,
                'E', _ => 'z' - 'a',
                _ => c - 'a'
            )
        )));
    }

    public record Node(char Character, Vector Position, int Height);
    public record Graph(IEnumerable<Node> Nodes, ILookup<Node, Node> Edges);
}