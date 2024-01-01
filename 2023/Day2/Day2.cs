using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public partial class Day2 : IDay
{
    private int red = 12;
    private int green = 13;
    private int blue = 14;

    record Game(int Id, int Reds, int Blues, int Greens);

    public long Part1(string input)
    {
        return input
            .Split(Environment.NewLine)
            .Select(ParseGame)
            .Where(g => g.Reds <= red && g.Blues <= blue && g.Greens <= green)
            .Sum(g => g.Id);
    }

    public long Part2(string input)
    {
        return input
            .Split(Environment.NewLine)
            .Select(ParseGame)
            .Sum(g => g.Reds * g.Blues * g.Greens);
    }

    private Game ParseGame(string line)
    {
        int gameId = int.Parse(GameIdRegex().Match(line).Groups[1].Value);
        var maxReds = RedRegex().Matches(line).Max(m => int.Parse(m.Groups[1].Value));
        var maxBlues = BlueRegex().Matches(line).Max(m => int.Parse(m.Groups[1].Value));
        var maxGreens = GreenRegex().Matches(line).Max(m => int.Parse(m.Groups[1].Value));
        return new Game(gameId, maxReds, maxBlues, maxGreens);
    }

    [GeneratedRegex(@"^Game (\d+):")]
    private partial Regex GameIdRegex();
    [GeneratedRegex(@" (\d+) red")]
    private partial Regex RedRegex();
    [GeneratedRegex(@" (\d+) blue")]
    private partial Regex BlueRegex();
    [GeneratedRegex(@" (\d+) green")]
    private partial Regex GreenRegex();
}
