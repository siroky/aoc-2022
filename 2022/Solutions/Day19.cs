namespace AOC.Solutions;

public class Day19 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var blueprints = lines.Select(p => ParseBlueprint(p)).ToList();
        var initialState = new State(
            Minerals: new MineralCounts(),
            Robots: new MineralCounts(Ore: 1),
            BuildBan: new HashSet<Mineral>()
        );

        var qualityLevels = blueprints.Select((b, i) => Solve(b, initialState, time: 24, max: 0) * (i + 1)).ToList();
        var goedeCounts = blueprints.Take(3).Select(b => Solve(b, initialState, time: 32, max: 0)).ToList();

        yield return qualityLevels.Sum().ToString();
        yield return goedeCounts.Product().ToString();
    }

    private int Solve(Blueprint blueprint, State state, int time, int max)
    {
        if (time == 0)
        {
            return state.Minerals.Geode;
        }
        if (!HasPotential(blueprint, state, time, max))
        {
            return 0;
        }

        var states = new List<State>();
        var buildable = new HashSet<Mineral>();
        var minedMinerals = Add(state.Minerals, state.Robots);

        foreach (var (mineral, costs) in blueprint.Robots)
        {
            if (LessOrEquals(costs, state.Minerals))
            {
                buildable.Add(mineral);

                if (!state.BuildBan.Contains(mineral))
                {
                    states.Add(new State(
                        Minerals: Add(minedMinerals, Invert(costs)),
                        Robots: Add(state.Robots, mineral, 1),
                        BuildBan: new HashSet<Mineral>()
                    ));
                }
            }
        }
        states.Add(new State(
            Minerals: minedMinerals,
            Robots: state.Robots,
            BuildBan: buildable
        ));

        var result = 0;
        foreach (var s in states)
        {
            var solution = Solve(blueprint, s, time - 1, max);
            result = Math.Max(result, solution);
            max = Math.Max(max, result);
        }

        return result;
    }

    private bool HasPotential(Blueprint blueprint, State state, int time, int max)
    {
        // Not enough time to even get to a single geode.
        if ((time == 1 && state.Robots.Geode == 0) ||
            (time == 3 && state.Robots.Obsidian == 0) ||
            (time == 5 && state.Robots.Clay == 0))
        {
            return false;
        }

        // Calculate tenative amount of produced geodes assuming infinite ore and parallel factory.
        // If the number of geodes produced in such conditions is smaller than current maximum, this state doesn't have potential. 
        var clay = state.Minerals.Clay + TentativeMineral(Math.Max(0, time - 4), state.Robots.Clay, Int32.MaxValue, blueprint.Robots[Mineral.Clay].Ore);
        var obsidian = state.Minerals.Obsidian + TentativeMineral(Math.Max(0, time - 2), state.Robots.Obsidian, clay, blueprint.Robots[Mineral.Obsidian].Clay);
        var geodes = state.Minerals.Geode + TentativeMineral(time, state.Robots.Geode, obsidian, blueprint.Robots[Mineral.Geode].Obsidian);
        if (geodes <= max)
        {
            return false;
        }

        return true;
    }

    private int TentativeMineral(int time, int guaranteedRobots, int dependentMineral, int cost)
    {
        var tentativeRobots = dependentMineral / cost;
        var min = Math.Min(tentativeRobots, time);
        var ramp = min * (min - 1) / 2;
        var rest = tentativeRobots * Math.Max(0, time - tentativeRobots);
        var tentativeMineral = ramp + rest;

        return guaranteedRobots * time + tentativeMineral;
    }

    private bool LessOrEquals(MineralCounts a, MineralCounts b)
    {
        return
            a.Ore <= b.Ore &&
            a.Clay <= b.Clay &&
            a.Obsidian <= b.Obsidian &&
            a.Geode <= b.Geode;
    }

    private MineralCounts Add(MineralCounts a, MineralCounts b)
    {
        return new MineralCounts(
            Ore: a.Ore + b.Ore,
            Clay: a.Clay + b.Clay,
            Obsidian: a.Obsidian + b.Obsidian,
            Geode: a.Geode + b.Geode
        );
    }

    private MineralCounts Add(MineralCounts a, Mineral mineral, int value)
    {
        return Add(a, new MineralCounts(
            Ore: mineral == Mineral.Ore ? value : 0,
            Clay: mineral == Mineral.Clay ? value : 0,
            Obsidian: mineral == Mineral.Obsidian ? value : 0,
            Geode: mineral == Mineral.Geode ? value : 0
        ));
    }

    private MineralCounts Invert(MineralCounts a)
    {
        return new MineralCounts(
            Ore: -a.Ore,
            Clay: -a.Clay,
            Obsidian: -a.Obsidian,
            Geode: -a.Geode
        );
    }

    private Blueprint ParseBlueprint(string line)
    {
        var sentences = line.Split(".", StringSplitOptions.RemoveEmptyEntries);
        var oreWords = sentences.First().Words().Reverse();
        var clayWords = sentences.Second().Words().Reverse();
        var obsidianWords = sentences.Third().Words().Reverse();
        var geodeWords = sentences.Fourth().Words().Reverse();

        return new Blueprint(new Dictionary<Mineral, MineralCounts>
        {
            { Mineral.Geode, new MineralCounts(Ore: geodeWords.Fifth().ToInt(), Obsidian: geodeWords.Second().ToInt()) },
            { Mineral.Obsidian, new MineralCounts(Ore: obsidianWords.Fifth().ToInt(), Clay: obsidianWords.Second().ToInt()) },
            { Mineral.Clay, new MineralCounts(Ore: clayWords.Second().ToInt()) },
            { Mineral.Ore, new MineralCounts(Ore: oreWords.Second().ToInt()) }
        });
    }

    private record struct MineralCounts(int Ore = 0, int Clay = 0, int Obsidian = 0, int Geode = 0);
    private record struct State(MineralCounts Minerals, MineralCounts Robots, HashSet<Mineral> BuildBan);
    private record Blueprint(Dictionary<Mineral, MineralCounts> Robots);

    private enum Mineral
    {
        Ore,
        Clay,
        Obsidian,
        Geode
    }
}