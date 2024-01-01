namespace AdventOfCode2023;

public partial class Day6 : IDay
{
    bool Winnable(int holdingTime, long recordTime, long recordDistance)
        => holdingTime * (recordTime - holdingTime) > recordDistance;

    public long Part1(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var times = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).ToList();
        var distances = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).ToList();

        return times.Select((t, idx) =>
        {
            int numWaysToWin = 0;
            // for each possible holding time (no 0 or t)
            for (int i = 1; i < t - 1; i++)
            {
                if (Winnable(i, t, distances[idx]))
                    numWaysToWin++;
            }

            return numWaysToWin;
        })
        .Aggregate((x, y) => x * y);
    }

    public long Part2(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var time = long.Parse(lines[0].Split(":", StringSplitOptions.TrimEntries)[1].Replace(" ", ""));
        var distance = long.Parse(lines[1].Split(":", StringSplitOptions.TrimEntries)[1].Replace(" ", ""));

        int numWaysToWin = 0;
        for (int i = 14; i < time - 14; i++)
        {
            if (Winnable(i, time, distance))
                numWaysToWin++;
        }

        return numWaysToWin;
    }
}