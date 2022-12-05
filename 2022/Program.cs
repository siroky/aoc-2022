using AOC.Solutions;

namespace AOC;

internal static class Program
{
    static void Main(string[] args)
    {
        for (var day = 1; day <= 25; day++)
        {
            var solverType = Type.GetType($"{nameof(AOC)}.{nameof(Solutions)}.Day{day}").ToOption();
            var solver = solverType.Map(t => (ISolver)Activator.CreateInstance(t));

            solver.Match(s => Solve(day, s));                
        }
    }

    private static void Solve(int day, ISolver solver)
    {
        var input = File.ReadAllLines($"Inputs/{day}.txt");
        var results = solver.Solve(input);
        var outputs = results.Select((r, i) => $"{day}.{i + 1} - {r}");
            
        foreach (var output in outputs)
        {
            Console.WriteLine(output);
        }
    }
}