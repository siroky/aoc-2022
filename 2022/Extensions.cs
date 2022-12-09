namespace AOC;

public static class Extensions
{
    public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        var partition = new List<T>();
        foreach (var item in items)
        {
            if (predicate(item) && partition.Any())
            {
                yield return partition;
                partition = new List<T>();
            }

            partition.Add(item);
        }

        if (partition.Any())
        {
            yield return partition;
        }
    }

    public static IEnumerable<T> Generate<T>(this int count, Func<int, T> item)
    {
        return Enumerable.Range(0, count).Select(i => item(i));
    }

    public static bool IsBlank(this string s)
    {
        return String.IsNullOrWhiteSpace(s);
    }

    public static int ToInt(this string s)
    {
        return Int32.Parse(s);
    }

    public static int ToInt(this char c)
    {
        return c.ToString().ToInt();
    }
}
