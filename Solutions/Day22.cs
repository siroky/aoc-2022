namespace AOC.Solutions;

public class Day22 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var paragraphs = lines.Paragraphs();
        var map = ParseMap(paragraphs.First());
        var commands = ParseCommands(paragraphs.Second().First());

        var folding1 = new Folding(new Dictionary<(Vector, Vector), (Vector, Vector, bool)>());
        var password1 = ExecuteCommands(map, folding1, commands);

        // TODO infer folding programmatically.
        var folding2 = new Folding(new Dictionary<(Vector, Vector), (Vector, Vector, bool)>
        {
            { (new Vector(1, 0), Vector.Up), (new Vector(0, 3), Vector.Right, false) },
            { (new Vector(1, 0), Vector.Left), (new Vector(0, 2), Vector.Right, true) },

            { (new Vector(2, 0), Vector.Up), (new Vector(0, 3), Vector.Up, false) },
            { (new Vector(2, 0), Vector.Down), (new Vector(1, 1), Vector.Left, false) },
            { (new Vector(2, 0), Vector.Right), (new Vector(1, 2), Vector.Left, true) },

            { (new Vector(1, 1), Vector.Left), (new Vector(0, 2), Vector.Down, false) },
            { (new Vector(1, 1), Vector.Right), (new Vector(2, 0), Vector.Up, false) },

            { (new Vector(0, 2), Vector.Up), (new Vector(1, 1), Vector.Right, false) },
            { (new Vector(0, 2), Vector.Left), (new Vector(1, 0), Vector.Right, true) },

            { (new Vector(1, 2), Vector.Down), (new Vector(0, 3), Vector.Left, false) },
            { (new Vector(1, 2), Vector.Right), (new Vector(2, 0), Vector.Left, true) },

            { (new Vector(0, 3), Vector.Down), (new Vector(2, 0), Vector.Down, false) },
            { (new Vector(0, 3), Vector.Left), (new Vector(1, 0), Vector.Down, false) },
            { (new Vector(0, 3), Vector.Right), (new Vector(1, 2), Vector.Up, false) }
        });
        var password2 = ExecuteCommands(map, folding2, commands);

        yield return password1.ToString();
        yield return password2.ToString();
    }

    private long ExecuteCommands(Map map, Folding folding, IEnumerable<Command> commands)
    {
        var positions = map.Tiles.Keys;
        var minY = positions.Min(p => p.Y);
        var initialX = positions.Where(p => p.Y == minY && !map.Tiles[p]).Min(p => p.X);

        var state = new State(
            Position: new Vector(initialX, minY),
            Direction: Vector.Right
        );

        foreach (var command in commands)
        {
            state = ExecuteCommand(map, folding, state, command);
        }

        return 1000 * (state.Position.Y + 1) + 4 * (state.Position.X + 1) + state.Direction.Match(
            Vector.Up, _ => 3,
            Vector.Down, _ => 1,
            Vector.Left, _ => 2,
            Vector.Right, _ => 0
        );
    }

    private State ExecuteCommand(Map map, Folding folding, State state, Command command)
    {
        return command.Match(
            turn => new State(
                Position: state.Position,
                Direction: state.Direction.Match(
                    Vector.Up, _ => turn.Right.Match(t => Vector.Right, f => Vector.Left),
                    Vector.Down, _ => turn.Right.Match(t => Vector.Left, f => Vector.Right),
                    Vector.Left, _ => turn.Right.Match(t => Vector.Up, f => Vector.Down),
                    Vector.Right, _ => turn.Right.Match(t => Vector.Down, f => Vector.Up)
                )
            ),
            walk => Enumerable.Range(0, walk.Steps).Aggregate(state, (s, _) => ExecuteStep(map, folding, s))
        );
    }

    private State ExecuteStep(Map map, Folding folding, State state)
    {
        var position = state.Position;
        var direction = state.Direction;
        var nextPosition = position.Add(direction);
        var nextDirection = direction;

        if (!map.Tiles.ContainsKey(nextPosition))
        {
            if (folding.SideMapping.IsEmpty())
            {
                var otherDirection = direction.Invert();
                var alternatives = map.Tiles.Keys.Where(p => p.Subtract(position).Sign() == otherDirection);
                nextPosition = alternatives.MaxBy(p => p.ManhattanDistance(position));
            }
            else
            {
                var side = position.Divide(map.SideSize);
                var (nextSide, newDirection, invert) = folding.SideMapping[(side, direction)];
                var cornerPosition = nextSide.Multiply(map.SideSize);
                var sidePosition = position.Remainder(map.SideSize);
                var sideMax = map.SideSize - 1;
                if (invert)
                {
                    sidePosition = new Vector(sideMax, sideMax).Subtract(sidePosition);
                }

                var sideCoordinate = direction.X == 0 ? sidePosition.X : sidePosition.Y;
                nextDirection = newDirection;
                nextPosition = cornerPosition.Add(newDirection.Match(
                    Vector.Up, _ => new Vector(sideCoordinate, sideMax),
                    Vector.Down, _ => new Vector(sideCoordinate, 0),
                    Vector.Left, _ => new Vector(sideMax, sideCoordinate),
                    Vector.Right, _ => new Vector(0, sideCoordinate)
                ));
            }
        }

        var nextIsWall = map.Tiles.Get(nextPosition).Get();
        return nextIsWall.Match(
            t => state,
            f => new State(nextPosition, nextDirection)
        );
    }

    private Map ParseMap(IEnumerable<string> lines)
    {
        var tiles = new Dictionary<Vector, bool>();
        for (var y = 0; y < lines.Count(); y++)
        {
            var line = lines.ElementAt(y);
            for (var x = 0; x < line.Length; x++)
            {
                line[x].Match(
                    '.', _ => tiles.Add(new Vector(x, y), false),
                    '#', _ => tiles.Add(new Vector(x, y), true),
                    ' ', _ => { }
                );
            }
        }

        var max = Math.Max(tiles.Keys.Max(p => p.X), tiles.Keys.Max(p => p.Y));
        var sideSize = (max + 1) / 4;

        return new Map(
            SideSize: sideSize,
            Tiles: tiles
        );
    }

    private IEnumerable<Command> ParseCommands(string line)
    {
        for (var i = 0; i < line.Length; i++)
        {
            yield return line[i].Match(
                'R', _ => new Command(new Turn(Right: true)),
                'L', _ => new Command(new Turn(Right: false)),
                _ =>
                {
                    var steps = String.Concat(line.Skip(i).TakeWhile(c => c != 'R' && c != 'L'));
                    i += steps.Length - 1;
                    return new Command(new Walk(steps.ToInt()));
                }
            );
        }
    }

    private record Map(long SideSize, Dictionary<Vector, bool> Tiles);
    private record State(Vector Position, Vector Direction);
    private record Folding(Dictionary<(Vector, Vector), (Vector, Vector, bool)> SideMapping);

    private record Turn(bool Right);
    private record Walk(int Steps);
    private class Command : Coproduct2<Turn, Walk>
    {
        public Command(Turn t) : base(t) { }
        public Command(Walk w) : base(w) { }
    }
}