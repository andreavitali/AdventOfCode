using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public partial class Day5 : IDay
{
    [GeneratedRegex(@"[a-z\- ]+\:")]
    private static partial Regex MapsRegex();

    public long Part1(string input)
    {
        var data = MapsRegex().Split(input.Replace("seeds: ", ""))
            .Select(l => l.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(mapLine => mapLine.Split(" ").Select(long.Parse).ToList())
                .ToList())
            .ToList();

        return Enumerable.Range(0, data[0][0].Count)
            .Select(idx =>
            {
                return Enumerable.Range(1, data.Count - 1)
                    .Aggregate(data[0][0][idx], (mapped, mapIdx) =>
                    {
                        for (int j = 0; j < data[mapIdx].Count; j++)
                        {
                            if (mapped >= data[mapIdx][j][1] && mapped < data[mapIdx][j][1] + data[mapIdx][j][2])
                            {
                                return data[mapIdx][j][0] + (mapped - data[mapIdx][j][1]);
                            }
                        }
                        return mapped;
                    });
            })
            .Min();
    }

    private IEnumerable<long> SeedRange(long startValue, long length)
    {
        var limit = startValue + length;
        while (startValue < limit)
        {
            yield return startValue;
            startValue++;
        }
    }

    public long Part2(string input)
    {
        var data = MapsRegex().Split(input.Replace("seeds: ", ""))
            .Select(l => l.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(mapLine => mapLine.Split(" ").Select(long.Parse).ToList())
                .ToList())
            .ToList();

        var seeds = data[0][0]
            .Select((_, i) => i % 2 == 0 ? SeedRange(_, data[0][0][i + 1]) : [])
            .SelectMany(r => r);

        return seeds
            .Select(s =>
            {
                return Enumerable.Range(1, data.Count - 1)
                    .Aggregate(s, (mapped, mapIdx) =>
                    {
                        for (int j = 0; j < data[mapIdx].Count; j++)
                        {
                            if (mapped >= data[mapIdx][j][1] && mapped < data[mapIdx][j][1] + data[mapIdx][j][2])
                            {
                                return data[mapIdx][j][0] + (mapped - data[mapIdx][j][1]);
                            }
                        }
                        return mapped;
                    });
            })
            .Min();
    }
}