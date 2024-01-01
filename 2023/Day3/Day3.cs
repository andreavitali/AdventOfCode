using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public partial class Day3 : IDay
{
    record EnginePart(int lineIndex, int columnIndex, int number, int endIndex);
    record Gear(int lineIndex, int columnIndex)
    {
        public bool IsAdjTo(EnginePart ep)
        {
            return (lineIndex == ep.lineIndex && (columnIndex == ep.columnIndex - 1 || columnIndex == ep.endIndex + 1)) ||
                (lineIndex == ep.lineIndex - 1 && columnIndex >= ep.columnIndex - 1 && columnIndex <= ep.endIndex + 1) ||
                (lineIndex == ep.lineIndex + 1 && columnIndex >= ep.columnIndex - 1 && columnIndex <= ep.endIndex + 1);
        }
    }

    private IEnumerable<EnginePart> ParseEngineParts(string line, int lineIndex)
    {
        return EnginePartRegex().Matches(line)
            .Select(m => new EnginePart(
                lineIndex,
                m.Groups[1].Index,
                int.Parse(m.Groups[1].Value),
                m.Groups[1].Index + m.Groups[1].Value.Length - 1
        ));
    }

    private IEnumerable<Gear> ParseGears(string[] lines)
    {
        for (int row = 0; row < lines.Length; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                if (lines[row][col] == '*')
                {
                    yield return new Gear(row, col);
                }
            }
        }
    }

    private bool isSymbol(char c) => !char.IsDigit(c) && c != '.';

    public long Part1(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var engineParts = lines.SelectMany(ParseEngineParts);
        var grid = lines.Select(l => l.ToCharArray()).ToArray();
        return engineParts
            .Where(ep =>
            {
                bool isValid = false;

                if (ep.columnIndex > 0)
                {
                    isValid = isSymbol(grid[ep.lineIndex][ep.columnIndex - 1]);
                }

                if (!isValid && ep.endIndex < lines[0].Length - 1)
                {
                    isValid = isSymbol(grid[ep.lineIndex][ep.endIndex + 1]);
                }

                if (!isValid && ep.lineIndex > 0)
                {
                    isValid = grid[ep.lineIndex - 1]
                        .Where((c, idx) => idx >= ep.columnIndex - 1 && idx <= ep.endIndex + 1 && isSymbol(c)).Any();
                }

                if (!isValid && ep.lineIndex >= 0 && ep.lineIndex < lines.Length - 1)
                {
                    isValid = grid[ep.lineIndex + 1]
                        .Where((c, idx) => idx >= ep.columnIndex - 1 && idx <= ep.endIndex + 1 && isSymbol(c)).Any();
                }

                return isValid;
            })
            .Sum(ep => ep.number);
    }

    public long Part2(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var engineParts = lines.SelectMany(ParseEngineParts);

        return ParseGears(lines)
            .Select(g => (gear: g, engines: engineParts.Where(ep => g.IsAdjTo(ep))))
            .Where(t => t.engines.Count() == 2)
            .Sum(t => t.engines.Aggregate(1, (acc, e) => acc * e.number));
    }

    [GeneratedRegex(@"(\d+)")]
    private partial Regex EnginePartRegex();
}