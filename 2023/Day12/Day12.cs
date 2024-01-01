using System.Text;

namespace AdventOfCode2023;

public partial class Day12 : IDay
{
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

    public long Part1(string input)
    {
        int lineIdx = 1;
        return input.Split(Environment.NewLine).Sum(l =>
        {
            System.Console.WriteLine("Line " + lineIdx);
            var sum = CountArrangementsBruteForce(l.Split(" ")[0], l.Split(" ")[1]);
            lineIdx++;
            return sum;
        });
    }


    public long Part2(string input)
    {
        int lineIdx = 1;
        return input.Split(Environment.NewLine)
        .Sum(l =>
        {
            System.Console.WriteLine("Line " + lineIdx);
            string springs = string.Join("?", Enumerable.Repeat(l.Split(" ")[0], 5));
            string descriptions = string.Join(",", Enumerable.Repeat(l.Split(" ")[1], 5));
            lineIdx++;
            return 0;
        });
    }
}