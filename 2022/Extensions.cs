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

    public static bool IsEmpty<T>(this IEnumerable<T> items)
    {
        return !items.Any();
    }

    public static bool IsSingle<T>(this IEnumerable<T> items)
    {
        return items.Take(2).Count() == 1;
    }

    public static bool IsMultiple<T>(this IEnumerable<T> items)
    {
        return items.Take(2).Count() == 2;
    }

    public static T Second<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(1);
    }

    public static T Third<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(2);
    }

    public static T Fourth<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(3);
    }

    public static T Fifth<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(4);
    }

    public static T Sixth<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(5);
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item)
    {
        return items.Concat(item.ToEnumerable());
    }

    public static int Count<T>(this IEnumerable<T> items, T item)
    {
        return items.Count(i => EqualityComparer<T>.Default.Equals(i, item));
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
        return s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    public static int Product(this IEnumerable<int> items)
    {
        return items.Aggregate((a, b) => a * b);
    }

    public static long Product(this IEnumerable<long> items)
    {
        return items.Aggregate((a, b) => a * b);
    }

    public static IEnumerable<IEnumerable<string>> Paragraphs(this IEnumerable<string> lines)
    {
        return lines.Partition(l => l.IsBlank()).Select(p => p.Where(l => !l.IsBlank()));
    }

    public static bool IsBlank(this string s)
    {
        return String.IsNullOrWhiteSpace(s);
    }

    public static int ToInt(this string s)
    {
        return Int32.Parse(s);
    }

    public static IOption<int> ToIntOption(this string s)
    {
        return Int32.TryParse(s, out var i).Match(
            t => i.ToOption(),
            f => Option.Empty<int>()
        );
    }

    public static int ToInt(this char c)
    {
        return c.ToString().ToInt();
    }

    public static long ToLong(this string s)
    {
        return Int64.Parse(s);
    }

    public static IOption<long> ToLongOption(this string s)
    {
        return Int64.TryParse(s, out var i).Match(
            t => i.ToOption(),
            f => Option.Empty<long>()
        );
    }
}
