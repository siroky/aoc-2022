using AOC.Solutions;

namespace AOC;

internal static class Program
{
    static void Main(string[] args)
    {
        for (var day = 25; day >= 1; day--)
        {
            var solverType = Type.GetType($"{nameof(AOC)}.{nameof(Solutions)}.Day{day}").ToOption();
            var solver = solverType.Map(t => (ISolver)Activator.CreateInstance(t)).GetOrNull();
            if (solver != null)
            {
                Solve(day, solver);
                break;
            }             
        }
    }

    private static void Solve(int day, ISolver solver)
    {
        var input = System.IO.File.ReadAllLines($"Inputs/{day}.txt");
        var results = solver.Solve(input);
        var outputs = results.Select((r, i) => $"{day}.{i + 1} - {r}");
            
        foreach (var output in outputs)
        {
            Console.WriteLine(output);
        }
    }
}