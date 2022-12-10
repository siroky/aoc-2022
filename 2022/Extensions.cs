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

    public static T Second<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(1);
    }

    public static T Third<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(2);
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item)
    {
        return items.Concat(item.ToEnumerable());
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items)
    {
        return items.SelectMany(i => i);
    }

    public static IEnumerable<T> Generate<T>(this int count, Func<int, T> item)
    {
        return Enumerable.Range(0, count).Select(i => item(i));
    }

    public static IEnumerable<T> ToEnumerable<T>(this T item)
    {
        return new[] { item };
    }

    public static IEnumerable<string> Words(this string s)
    {
        return s.Split(' ');
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
