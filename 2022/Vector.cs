namespace AOC;

public record struct Vector(long X, long Y)
{
    public static readonly Vector Zero = new Vector(0, 0);

    public static readonly IEnumerable<Vector> Basis = new[]
    {
        new Vector(1, 0),
        new Vector(0, 1)
    };

    public Vector Add(Vector b)
    {
        return new Vector(X + b.X, Y + b.Y);
    }

    public Vector AddX(long value)
    {
        return new Vector(X + value, Y);
    }

    public Vector AddY(long value)
    {
        return new Vector(X, Y + value);
    }

    public Vector Subtract(Vector b)
    {
        return Add(b.Invert());
    }

    public Vector Invert()
    {
        return new Vector(-X, -Y);
    }

    public Vector Sign()
    {
        return new Vector(Math.Sign(X), Math.Sign(Y));
    }

    public long ManhattanDistance(Vector b)
    {
        return Math.Abs(X - b.X) + Math.Abs(Y - b.Y);
    }

    public double Length()
    {
        return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
    }
}
