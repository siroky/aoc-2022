using FuncSharp;

namespace AOC.Solutions;

public class Day2 : ISolver
{
    public IEnumerable<int> Solve(IEnumerable<string> input)
    {
        yield return input.Select(i => ScoreStrategy1(i)).Sum();
        yield return input.Select(i => ScoreStrategy2(i)).Sum();
    }

    private int ScoreStrategy1(string line)
    {
        var opponent = ParseOpponent(line[0]);
        var player = ParsePlayer(line[2]);
        var result = Game.Results.Get(player, opponent).Get();

        return (int)player + (int)result;
    }

    private int ScoreStrategy2(string line)
    {
        var opponent = ParseOpponent(line[0]);
        var result = ParseResult(line[2]);
        var player = Game.Players.Get(opponent, result).Get();

        return (int)player + (int)result;
    }

    private Move ParseOpponent(char c)
    {
        return c.Match(
            'A', _ => Move.Rock,
            'B', _ => Move.Paper,
            'C', _ => Move.Scissors
        );
    }

    private Move ParsePlayer(char c)
    {
        return c.Match(
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
}

public static class Game
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

public enum Move
{
    Rock = 1,
    Paper = 2,
    Scissors = 3
}

public enum Result
{
    Loss = 0,
    Draw = 3,
    Win = 6
}
