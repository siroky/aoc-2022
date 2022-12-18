namespace AOC;

public record struct Vector(long X, long Y, long Z = 0)
{
    public static readonly Vector Zero = new Vector(0, 0, 0);

    public static readonly Vector Unit = new Vector(1, 1, 1);

    public static readonly IEnumerable<Vector> Basis2 = new[]
    {
        new Vector(1, 0, 0),
        new Vector(0, 1, 0)
    };

    public static Vector Max(IEnumerable<Vector> vectors)
    {
        return new Vector(
            vectors.Max(c => c.X),
            vectors.Max(c => c.Y),
            vectors.Max(c => c.Z)
        );
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

    public long ManhattanDistance(Vector b)
    {
        return Math.Abs(X - b.X) + Math.Abs(Y - b.Y) + Math.Abs(Z - b.Z);
    }

    public bool PreceedsOrEquals(Vector b)
    {
        return X <= b.X && Y <= b.Y && Z <= b.Z;
    }

    public double Length()
    {
        return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
    }
}
