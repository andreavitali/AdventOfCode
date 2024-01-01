namespace AdventOfCode2023;

public partial class Day9 : IDay
{
    public long Part1(string input)
    {
        return input.Split(Environment.NewLine)
            .Select(l => l.Split(" ").Select(int.Parse).ToList())
            .Select(GetTotalValue)
            .Sum();
    }

    public long Part2(string input)
    {
        return input.Split(Environment.NewLine)
            .Select(l => l.Split(" ").Select(int.Parse).ToList())
            .Select(h => h.Reverse<int>())
            .Select(GetTotalValue)
            .Sum();
    }

    private int GetTotalValue(IEnumerable<int> history)
    {
        int total = 0;
        var current = history;
        while (!current.All(c => c == 0))
        {
            total += current.Last();
            current = current.Zip(current.Skip(1), (a, b) => b - a).ToList();
        }
        return total;
    }
}