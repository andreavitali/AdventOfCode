namespace AdventOfCode2023;

public partial class Day4 : IDay
{
    public long Part1(string input)
    {
        return input.Split(Environment.NewLine)
            .Sum(line =>
            {
                var values = line.Split(":")[1].Split("|");
                var winning = values[0].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                var owned = values[1].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                return (int)Math.Pow(2, winning.Intersect(owned).Count() - 1);
            });
    }

    public long Part2(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var cardCount = lines.Select((_, idx) => idx).ToDictionary(i => i, _ => 1);

        int lIdx = 0;
        foreach (string line in lines)
        {
            var values = line.Split(":")[1].Split("|");
            var winning = values[0].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var owned = values[1].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            int winCopies = winning.Intersect(owned).Count();
            for (int i = 1; i <= winCopies; i++)
                cardCount[lIdx + i] += cardCount[lIdx];
            lIdx++;
        }

        return cardCount.Values.Sum();
    }
}