namespace AOC.Solutions;

public class Day02 : ISolver
{
    public IEnumerable<string> Solve(IEnumerable<string> lines)
    {
        yield return lines.Sum(l => ScoreStrategy1(l)).ToString();
        yield return lines.Sum(l => ScoreStrategy2(l)).ToString();
    }

    private int ScoreStrategy1(string line)
    {
        var opponent = ParseMove(line[0]);
        var player = ParseMove(line[2]);
        var result = Game.Results.Get(player, opponent).Get();

        return (int)player + (int)result;
    }

    private int ScoreStrategy2(string line)
    {
        var opponent = ParseMove(line[0]);
        var result = ParseResult(line[2]);
        var player = Game.Players.Get(opponent, result).Get();

        return (int)player + (int)result;
    }

    private Move ParseMove(char c)
    {
        return c.Match(
            'A', _ => Move.Rock,
            'B', _ => Move.Paper,
            'C', _ => Move.Scissors,
            'X', _ => Move.Rock,
            'Y', _ => Move.Paper,
            'Z', _ => Move.Scissors
        );
    }
    private Result ParseResult(char c)
    {
        return c.Match(
            'X', _ => Result.Loss,
            'Y', _ => Result.Draw,
            'Z', _ => Result.Win
        );
    }

    private enum Move
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    private enum Result
    {
        Loss = 0,
        Draw = 3,
        Win = 6
    }

    private static class Game
    {
        public static IEnumerable<(Move, Move, Result)> States { get; }
        public static DataCube2<Move, Move, Result> Results { get; }
        public static DataCube2<Move, Result, Move> Players { get; }

        static Game()
        {
            States = new[]
            {
            (Move.Rock, Move.Rock, Result.Draw),
            (Move.Rock, Move.Paper, Result.Loss),
            (Move.Rock, Move.Scissors, Result.Win),

            (Move.Paper, Move.Rock, Result.Win),
            (Move.Paper, Move.Paper, Result.Draw),
            (Move.Paper, Move.Scissors, Result.Loss),

            (Move.Scissors, Move.Rock, Result.Loss),
            (Move.Scissors, Move.Paper, Result.Win),
            (Move.Scissors, Move.Scissors, Result.Draw)
        };

            Results = States.ToDataCube(s => s.Item1, s => s.Item2, s => s.Item3);
            Players = States.ToDataCube(s => s.Item2, s => s.Item3, s => s.Item1);
        }
    }
}