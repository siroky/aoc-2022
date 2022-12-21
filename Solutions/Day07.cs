namespace AOC.Solutions;

public class Day07 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        var commands = Console.ParseCommands(lines);
        var root = Interpret(commands);
        var smaller = FileSystem.Measure(root, size => size <= 100_000);

        var availableSpace = 70_000_000 - smaller.Size;
        var minimalSpace = 30_000_000;
        var requiredSpace = minimalSpace - availableSpace;
        var candidates = FileSystem.Measure(root, size => size >= requiredSpace);

        yield return smaller.Data.Sum().ToString();
        yield return candidates.Data.Min().ToString();
    }

    private FileSystem.Directory Interpret(IEnumerable<Console.Command> commands)
    {
        var root = new FileSystem.Directory("/", parent: null);
        var current = root;

        foreach (var command in commands)
        {
            command.Match(
                cd => current = cd.Name.Match(
                    "/", _ => root,
                    "..", _ => current.Parent,
                    _ => current.Directories.First(d => d.Name.Equals(cd.Name))
                ),
                ls =>
                {
                    current.Files.AddRange(ls.Files.Select(f => new FileSystem.File(f.Name, f.Size)));
                    current.Directories.AddRange(ls.Directories.Select(d => new FileSystem.Directory(d.Name, current)));
                }
            );
        }

        return root;
    }

    private static class FileSystem
    {
        public record File(string Name, int Size);
        public record Directory(string Name, Directory Parent, List<Directory> Directories, List<File> Files)
        {
            public Directory(string name, Directory parent) : this(name, parent, new List<Directory>(), new List<File>()) { }
        }

        public static MeasurementResult Measure(Directory directory, Func<int, bool> dataPredicate)
        {
            var childResults = directory.Directories.Select(d => Measure(d, dataPredicate)).ToList();
            var childSizes = childResults.Sum(m => m.Size);
            var fileSizes = directory.Files.Sum(f => f.Size);
            var size = childSizes + fileSizes;

            var childData = childResults.SelectMany(r => r.Data);
            var directoryData = size.ToOption().Where(s => dataPredicate(s)).ToEnumerable();
            var data = childData.Concat(directoryData).Aggregate(Enumerable.Empty<int>(), (a, b) => a.Concat(b));

            return new MeasurementResult(size, data);
        }

        public record MeasurementResult(int Size, IEnumerable<int> Data);
    }

    private static class Console
    {
        public record File(string Name, int Size);
        public record Directory(string Name);
        public record ChangeDirectory(string Name);
        public record ListDirectory(IEnumerable<File> Files, IEnumerable<Directory> Directories);
        public class Command : Coproduct2<ChangeDirectory, ListDirectory>
        {
            public Command(ChangeDirectory change) : base(change) { }
            public Command(ListDirectory list) : base(list) { }
        }

        public static IEnumerable<Command> ParseCommands(IEnumerable<string> lines)
        {
            var partitions = lines.Partition(l => l.StartsWith('$'));
            return partitions.Select(p => ParseCommand(p));
        }

        private static Command ParseCommand(IEnumerable<string> lines)
        {
            var words = lines.First().Words();

            return words.Second().Match(
                "cd", _ => new Command(new ChangeDirectory(words.Third())),
                "ls", _ => new Command(ParseListDirectory(lines.Skip(1)))
            );
        }

        private static ListDirectory ParseListDirectory(IEnumerable<string> contents)
        {
            var files = new List<File>();
            var directories = new List<Directory>();
            
            foreach (var line in contents)
            {
                var words = line.Words();
                var name = words.Second();

                words.First().Match(
                    "dir", _ => directories.Add(new Directory(name)),
                    size => files.Add(new File(name, size.ToInt()))
                );
            }

            return new ListDirectory(files, directories);
        }
    }
}