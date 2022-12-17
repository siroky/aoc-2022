namespace AOC.Solutions;

public class Day17 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var cave = new Cave(
            width: 7,
            jets: ParseJets(lines.First()).ToList()
        );

        var (height1, _, _) = Simulate(cave, 2022);
        var (_, heights, rocks) = Simulate(cave, 1_000_000);

        // After first loop through all the jets, it becomes periodic every 1745 rocks with height increase of 2750.
        // This assumes it becomes periodic after first loop, but could be extended to smarter period detection...
        var heightDiffs = heights.Zip(heights.Skip(1)).Select(p => p.Second - p.First);
        var rockDiffs = rocks.Zip(rocks.Skip(1)).Select(p => p.Second - p.First);
        var periodHeight = heightDiffs.Second();
        var periodRocks = rockDiffs.Second();

        var rockCount = 1_000_000_000_000;
        var calculatedPeriods = rockCount / periodRocks - 10;
        var calculatedRocks = calculatedPeriods * periodRocks;
        var calculatedHeight = calculatedPeriods * periodHeight;

        var simulatedRocks = rockCount - calculatedRocks;
        var (simulatedHeight, _, _) = Simulate(cave, simulatedRocks);
        var height2 = calculatedHeight + simulatedHeight;

        yield return height1.ToString();
        yield return height2.ToString();
    }

    private (long Height, List<long> Heights, List<long> Rocks) Simulate(Cave cave, long rockCount)
    {
        var tower = new Tower(cave.Width);
        var heights = new List<long>();
        var rocks = new List<long>();

        for (long rockIndex = 0, step = 0; rockIndex < rockCount; rockIndex++)
        {
            if (step % cave.Jets.Count == 0)
            {
                heights.Add(tower.Height);
                rocks.Add(rockIndex);
            }

            var rock = cave.GetRock(rockIndex);
            step = SimulateRock(cave, tower, rock, step);
        }

        return (tower.Height, heights, rocks);
    }

    private long SimulateRock(Cave cave, Tower tower, Rock rock, long step)
    {
        // Fastpath first 3 steps which are always guaranteed to be successful.
        var x1 = 3;
        var x2 = x1 + rock.Width - 1;
        var jet1 = cave.GetJet(step++);
        var jet2 = cave.GetJet(step++);
        var jet3 = cave.GetJet(step++);

        if (jet1 < 0 || x2 < tower.Width)
        {
            x1 += jet1;
            x2 += jet1;
        }
        if (jet2 < 0 || x2 < tower.Width)
        {
            x1 += jet2;
            x2 += jet2;
        }
        if (jet3 < 0)
        {
            if (x1 > 1)
            {
                x1 += jet3;
            }
        }
        else
        {
            if (x2 < tower.Width)
            {
                x1 += jet3;
            }
        }

        // Simulate remaining steps.
        var x = x1;
        var y = tower.Height + 1;
        while (true)
        {
            var jet = cave.GetJet(step++);
            if (tower.Fits(rock, x + jet, y))
            {
                x += jet;
            }

            if (tower.Fits(rock, x, y - 1))
            {
                y -= 1;
            }
            else
            {
                tower.Add(rock, x, y);
                break;
            }
        }

        return step;
    }

    private IEnumerable<int> ParseJets(string line)
    {
        return line.ToCharArray().Select(j => j.Match(
            '<', _ => -1,
            '>', _ => 1
        ));
    }

    private class Tower
    {
        public Tower(long width)
        {
            Width = width;
            Height = 0;
            Occupancy = new bool[Width + 1, 400_000_000];
        }

        public long Width { get; private set; }
        public long Height { get; private set; }
        private bool[,] Occupancy { get; set; }

        public bool Fits(Rock rock, long x, long y)
        {
            if (x <= 0 || y <= 0 || x + rock.Width - 1 > Width)
            {
                return false;
            }
            return !rock.Collides(Occupancy, x, y);
        }

        public void Add(Rock rock, long x, long y)
        {
            var maxY = rock.Add(Occupancy, x, y);
            if (maxY > Height)
            {
                Height = maxY;
            }
        }
    }

    private class Cave
    {
        public Cave(int width, List<int> jets)
        {
            Width = width;
            Jets = jets;
            Rocks = new List<Rock>
            {
                new Rock(
                    Shape: "-",
                    Width: 4,
                    Collides: (o, x, y) => o[x, y] || o[x + 1, y] || o[x + 2, y] || o[x + 3, y],
                    Add: (o, x, y) =>
                    {
                        o[x, y] = true;
                        o[x + 1, y] = true;
                        o[x + 2, y] = true;
                        o[x + 3, y] = true;
                        return y;
                    }
                ),
                new Rock(
                    Shape: "+",
                    Width: 3,
                    Collides: (o, x, y) => o[x + 1, y] || o[x, y + 1] || o[x + 2, y + 1] || o[x + 1, y + 2],
                    Add: (o, x, y) =>
                    {
                        o[x + 1, y] = true;
                        o[x, y + 1] = true;
                        o[x + 1, y + 1] = true;
                        o[x + 2, y + 1] = true;
                        o[x + 1, y + 2]  = true;
                        return y + 2;
                    }
                ),
                new Rock(
                    Shape: "⅃",
                    Width: 3,
                    Collides: (o, x, y) => o[x, y] || o[x + 1, y] || o[x + 2, y] || o[x + 2, y + 1] || o[x + 2, y + 2],
                    Add: (o, x, y) =>
                    {
                        o[x, y] = true;
                        o[x + 1, y] = true;
                        o[x + 2, y] = true;
                        o[x + 2, y + 1] = true;
                        o[x + 2, y + 2] = true;
                        return y + 2;
                    }
                ),
                new Rock(
                    Shape: "|",
                    Width: 1,
                    Collides: (o, x, y) => o[x, y] || o[x, y + 1] || o[x, y + 2] || o[x, y + 3],
                    Add: (o, x, y) =>
                    {
                        o[x, y] = true;
                        o[x, y + 1] = true;
                        o[x, y + 2] = true;
                        o[x, y + 3]  = true;
                        return y + 3;
                    }
                ),
                new Rock(
                    Shape: "□",
                    Width: 2,
                    Collides: (o, x, y) => o[x, y] || o[x + 1, y] || o[x, y + 1] || o[x + 1, y + 1],
                    Add: (o, x, y) =>
                    {
                        o[x, y] = true;
                        o[x + 1, y] = true;
                        o[x, y + 1] = true;
                        o[x + 1, y + 1] = true;
                        return y + 1;
                    }
                )
            };
        }

        public int Width { get; private set; }
        public List<int> Jets { get; private set; }
        public List<Rock> Rocks { get; private set; }

        public int GetJet(long step)
        {
            return Jets[(int)(step % Jets.Count)];
        }

        public Rock GetRock(long index)
        {
            return Rocks[(int)(index % Rocks.Count)];
        }
    }

    private record Rock(string Shape, int Width, Func<bool[,], long, long, bool> Collides, Func<bool[,], long, long, long> Add);
}