namespace AOC.Solutions;

public class Day20 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var ring1 = ParseRing(lines);
        Mix(ring1);

        var ring2 = ParseRing(lines, decryptionKey: 811589153);
        Mix(ring2, count: 10);

        yield return CoordinateValues(ring1).Sum().ToString();
        yield return CoordinateValues(ring2).Sum().ToString();
    }

    private IEnumerable<long> CoordinateValues(Ring ring)
    {
        var coords = new[] { 1000, 2000, 3000 };
        var zero = ring.Items.First(i => i.Value == 0);
        return coords.Select(c => Seek(ring, zero, c).Value);
    }

    private Item Seek(Ring ring, Item item, long count)
    {
        for (var i = 0; i < count % ring.Items.Count; i++)
        {
            item = item.Next;
        }
        return item;
    }

    private void Mix(Ring ring, long count = 1)
    {
        var items = ring.Items.ToList();
        var itemCount = items.Count;

        for (var mix = 0; mix < count; mix++)
        {
            foreach (var item in items)
            {
                for (var i = 0; i < Math.Abs(item.Value) % (itemCount - 1); i++)
                {
                    if (item.Value > 0)
                    {
                        SwapNext(ring, item);
                    }
                    else
                    {
                        SwapNext(ring, item.Previous);
                    }
                }
            }
        }
    }

    private void SwapNext(Ring ring, Item item)
    {
        var previous = item.Previous;
        var firstNext = item.Next;
        var secondNext = firstNext.Next;

        previous.Next = firstNext;
        firstNext.Previous = previous;
        firstNext.Next = item;
        item.Previous = firstNext;
        item.Next = secondNext;
        secondNext.Previous = item;

        if (ring.Start == firstNext)
        {
            ring.Start = item;
        }
        if (ring.Start == item)
        {
            ring.Start = firstNext;
        }
    }

    private Ring ParseRing(IEnumerable<string> lines, long decryptionKey = 1)
    {
        var ring = new Ring(new List<Item>());

        foreach (var line in lines)
        {
            var item = new Item(line.ToLong() * decryptionKey);

            if (ring.Items.IsEmpty())
            {
                item.Next = item.Previous = item;
                ring.Start = item;
            }
            else
            {
                var start = ring.Start;
                var last = start.Previous;

                last.Next = item;
                item.Previous = last;
                item.Next = start;
                start.Previous = item;
            }

            ring.Items.Add(item);
        }

        return ring;
    }

    private record Ring(List<Item> Items)
    {
        public Item Start { get; set; }
    }

    private record Item(long Value)
    {
        public Item Next { get; set; }
        public Item Previous { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}