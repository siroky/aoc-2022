namespace AOC;

public record struct Vector(long X, long Y, long Z = 0)
{
    public static readonly Vector Zero = new Vector(0, 0, 0);

    public static readonly Vector Unit = new Vector(1, 1, 1);

    public static Vector Max(IEnumerable<Vector> vectors)
    {
        return new Vector(
            vectors.Max(c => c.X),
            vectors.Max(c => c.Y),
            vectors.Max(c => c.Z)
        );
    }

    public static Vector Parse(IEnumerable<string> values)
    {
        return new Vector(
            values.First().ToLong(),
            values.Skip(1).FirstOption().Map(c => c.ToLong()).GetOrElse(0L),
            values.Skip(2).FirstOption().Map(c => c.ToLong()).GetOrElse(0L)
        );
    }

    public double Length()
    {
        return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
    }

    public long ManhattanDistance(Vector b)
    {
        return Math.Abs(X - b.X) + Math.Abs(Y - b.Y) + Math.Abs(Z - b.Z);
    }

    public bool LessOrEquals(Vector b)
    {
        return X <= b.X && Y <= b.Y && Z <= b.Z;
    }

    public Vector Add(Vector b)
    {
        return new Vector(X + b.X, Y + b.Y, Z + b.Z);
    }

    public Vector AddX(long value)
    {
        return new Vector(X + value, Y, Z);
    }

    public Vector AddY(long value)
    {
        return new Vector(X, Y + value, Z);
    }

    public Vector AddZ(long value)
    {
        return new Vector(X, Y, Z + value);
    }

    public Vector Subtract(Vector b)
    {
        return Add(b.Invert());
    }

    public Vector Invert()
    {
        return new Vector(-X, -Y, -Z);
    }

    public Vector Sign()
    {
        return new Vector(Math.Sign(X), Math.Sign(Y), Math.Sign(Z));
    }

    public IEnumerable<Vector> AdjacentX()
    {
        yield return AddX(1);
        yield return AddX(-1);
    }

    public IEnumerable<Vector> AdjacentY()
    {
        yield return AddY(1);
        yield return AddY(-1);
    }

    public IEnumerable<Vector> AdjacentZ()
    {
        yield return AddZ(1);
        yield return AddZ(-1);
    }

    public IEnumerable<Vector> AdjacentXY()
    {
        return AdjacentX().Concat(AdjacentY());
    }

    public IEnumerable<Vector> AdjacentXYZ()
    {
        return AdjacentXY().Concat(AdjacentZ());
    }
}
