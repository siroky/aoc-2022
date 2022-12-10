namespace AOC.Solutions;

public class Day10 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var instructions = ParseInstructions(lines);
        var registers = ExecuteInstructions(instructions).Flatten().ToList();

        var interestingCycles = Enumerable.Range(0, 6).Select(i => 20 + i * 40);
        var interestingStrengths = interestingCycles.Select(c => EvaluateStrength(registers, c));

        var screen = Render(registers);

        yield return interestingStrengths.Sum().ToString();
        yield return "\n" + String.Join("\n", screen);
    }

    private IEnumerable<IEnumerable<int>> ExecuteInstructions(IEnumerable<Instruction> instructions)
    {
        var register = 1;
        foreach (var instruction in instructions)
        {
            yield return instruction.Match(
                noop => register.ToEnumerable(),
                add =>
                {
                    var registers = Enumerable.Repeat(register, 2);
                    register += add.Value;
                    return registers;
                }
            );
        }
    }

    private int EvaluateStrength(IEnumerable<int> registers, int cycle)
    {
        return cycle * registers.ElementAt(cycle - 1);
    }

    private IEnumerable<string> Render(IEnumerable<int> registers)
    {
        foreach (var batch in registers.Chunk(40))
        {
            var pixels = batch.Select((register, index) => (Math.Abs(register - index) <= 1).Match(
                t => "█",
                f => " "
            ));
            
            yield return String.Concat(pixels);
        }
    }

    private IEnumerable<Instruction> ParseInstructions(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var words = line.Words();
            yield return words.First().Match(
                "noop", _ => new Instruction(new Noop()),
                "addx", _ => new Instruction(new Add(words.Second().ToInt()))
            );
        }
    }

    public record Noop();
    public record Add(int Value);
    public class Instruction : Coproduct2<Noop, Add>
    {
        public Instruction(Noop n) : base(n) { }
        public Instruction(Add a) : base(a) { }
    }
}