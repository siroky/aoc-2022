using System.Text.RegularExpressions;

namespace AOC.Solutions;

public class Day15 : ISolver
{
    private static readonly ComparableTotalOrder<long> Intervals = new ComparableTotalOrder<long>();

    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var sensors = lines.Select(l => ParseSensor(l)).ToList();
        var deadZoneSize = DeadZoneSize(sensors, 2000000);

        var beacon = FindBeaconPosition(sensors, 4000000);
        var beaconTuningFrequency = beacon.Map(b => b.X * 4000000L + b.Y);

        yield return deadZoneSize.ToString();
        yield return beaconTuningFrequency.GetOrDefault().ToString();
    }

    private IOption<Vector> FindBeaconPosition(IEnumerable<Sensor> sensors, int maxPosition)
    {
        var searchZone = Intervals.ClosedInterval(0, maxPosition);
        for (var y = 0; y <= maxPosition; y++)
        {
            var deadZone = DeadZone(sensors, y);
            var position = Intervals.Intersect(deadZone, searchZone);
            if (position.Intervals.IsMultiple())
            {
                var x = position.Intervals.First().UpperBoundValue.Get() + 1;
                return new Vector(x, y).ToOption();
            }
        }

        return Option.Empty<Vector>();
    }

    private long DeadZoneSize(IEnumerable<Sensor> sensors, int y)
    {
        var deadZone = DeadZone(sensors, y);
        var deadZoneSize = deadZone.Intervals.Sum(i => i.UpperBoundValue.Get() - i.LowerBoundValue.Get() + 1);

        var beacons = sensors.Select(s => s.Beacon).Distinct();
        var beaconCount = beacons.Count(b => b.Y == y);

        return deadZoneSize - beaconCount;
    }

    private IntervalSet<long> DeadZone(IEnumerable<Sensor> sensors, int y)
    {
        return Intervals.Union(sensors.Select(s => DeadZone(s, y)));
    }

    private Interval<long> DeadZone(Sensor sensor, int y)
    {
        var lineDistance = Math.Abs(sensor.Position.Y - y);
        var beaconDistance = sensor.Position.ManhattanDistance(sensor.Beacon);
        var coverage = beaconDistance - lineDistance;

        return (coverage >= 0).Match(
            t => Intervals.ClosedInterval(sensor.Position.X - coverage, sensor.Position.X + coverage),
            f => Intervals.EmptyInterval
        );
    }

    private Sensor ParseSensor(string line)
    {
        var words = line.Words();
        var indexes = new[] { 2, 3, 8, 9 };
        var coords = indexes.Select(i => Regex.Replace(words.ElementAt(i), "[,:xy=]+", ""));
        return new Sensor(
            Position: Vector.Parse(coords.Take(2)),
            Beacon: Vector.Parse(coords.Skip(2).Take(2))
        );
    }

    private record Sensor(Vector Position, Vector Beacon);
}