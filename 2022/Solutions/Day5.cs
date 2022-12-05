namespace AOC.Solutions;

public class Day5 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var stackLines = lines.TakeWhile(l => !String.IsNullOrWhiteSpace(l)).ToList();
        var commandLines = lines.Skip(stackLines.Count + 1);

        var stacks9000 = ParseStacks(stackLines);
        var stacks9001 = ParseStacks(stackLines);
        var commands = ParseCommands(commandLines);

        yield return ExecuteCommands(stacks9000, commands, batched: false);
        yield return ExecuteCommands(stacks9001, commands, batched: true);   
    }

    private string ExecuteCommands(List<Stack<char>> stacks, IEnumerable<Command> commands, bool batched)
    {
        foreach (var command in commands)
        {
            ExecuteCommand(stacks, command, batched);
        }

        var stackTops = stacks.Where(s => s.Any()).Select(s => s.Peek());
        return new string(stackTops.ToArray());
    }

    private void ExecuteCommand(List<Stack<char>> stacks, Command command, bool batched)
    {
        var source = stacks[command.SourceIndex];
        var target = stacks[command.TargetIndex];
        var boxes = Enumerable.Range(0, command.BoxCount).Select(_ => source.Pop());
        var payload = batched.Match(
            t => boxes.Reverse(),
            f => boxes
        );

        foreach (var box in payload)
        {
            target.Push(box);
        }
    }

    private List<Stack<char>> ParseStacks(IEnumerable<string> lines)
    {
        var stackLines = lines.Reverse();

        var counts = stackLines.First().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var count = Int32.Parse(counts.Last());
        var stacks = Enumerable.Range(0, count).Select(_ => new Stack<char>()).ToList();

        foreach (var line in stackLines.Skip(1))
        {
            for (var i = 0; i < count; i++)
            {
                var box = line[4 * i + 1];
                if (box != ' ')
                {
                    stacks[i].Push(box);
                }
            }
        }

        return stacks;
    }

    private IEnumerable<Command> ParseCommands(IEnumerable<string> commandLines)
    {
        foreach (var line in commandLines)
        {
            var parts = line.Split(' ');
            yield return new Command(
                BoxCount: Int32.Parse(parts[1]),
                SourceIndex: Int32.Parse(parts[3]) - 1,
                TargetIndex: Int32.Parse(parts[5]) - 1
            );
        }
    }
}

public record Command(int BoxCount, int SourceIndex, int TargetIndex);