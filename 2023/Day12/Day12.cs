using System.Text;
using Cache = System.Collections.Generic.Dictionary<(string, string), long>;

namespace AdventOfCode2023;

public partial class Day12 : IDay
{
    public long Part1(string input)
    {
        return input.Split(Environment.NewLine).Sum(l => CountArrangementsBruteForce(l.Split(" ")[0], l.Split(" ")[1]));
    }

    private int CountArrangementsBruteForce(string springs, string description)
    {
        if (!springs.Any(s => s == '?'))
        {
            var currentDescr = String.Join(",", springs.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(g => g.Length));
            return currentDescr == description ? 1 : 0;
        }

        int sum = 0;
        int firstQuestionMark = springs.IndexOf('?');
        StringBuilder sb = new StringBuilder(springs);
        sb[firstQuestionMark] = '#';
        sum += CountArrangementsBruteForce(sb.ToString(), description);
        sb[firstQuestionMark] = '.';
        sum += CountArrangementsBruteForce(sb.ToString(), description);
        return sum;
    }

    public long Part2(string input)
    {
        int lineIdx = 1;
        return input.Split(Environment.NewLine)
        .Sum(l =>
        {
            string springs = string.Join("?", Enumerable.Repeat(l.Split(" ")[0], 5));
            string groups = string.Join(",", Enumerable.Repeat(l.Split(" ")[1], 5));
            var numGroups = groups.Split(",").Select(int.Parse).ToList();
            var sum = CachedCount(springs, numGroups, new Cache());
            System.Console.WriteLine($"Line {lineIdx}: {sum}");
            lineIdx++;
            return sum;
        });
    }

    private long CachedCount(string springs, List<int> groups, Cache cache)
    {
        string gKey = String.Join("", groups.Select(g => g));
        if (!cache.ContainsKey((springs, gKey)))
        {
            cache[(springs, gKey)] = CountArrangements(springs, groups, cache);
        }
        return cache[(springs, gKey)];
    }

    private long CountArrangements(string springs, List<int> groups, Cache cache)
    {
        if (springs.Length == 0) return groups.Count == 0 ? 1 : 0;
        if (groups.Count == 0) return springs.Contains('#') ? 0 : 1;
        var firstChar = springs[0];

        if (firstChar == '.') return CachedCount(springs[1..], groups, cache);
        else if (firstChar == '#')
        {
            var group = groups[0];
            var potentiallyDead = springs.TakeWhile(s => s == '#' || s == '?').Count();
            if (potentiallyDead < group) return 0;
            else if (springs.Length == group) return CachedCount("", groups[1..], cache);
            else if (springs[group] == '#') return 0;
            else return CachedCount(springs[(group + 1)..], groups[1..], cache);
        }
        else // '?'
        {
            return CachedCount('#' + springs[1..], groups, cache) + CachedCount('.' + springs[1..], groups, cache);
        }
    }
}