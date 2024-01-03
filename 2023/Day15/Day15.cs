using System.Collections.Specialized;

namespace AdventOfCode2023;

public partial class Day15 : IDay
{
    public long Part1(string input)
    {
        return input.Split(",").Sum(CalcHash);
    }

    public long Part2(string input)
    {
        Dictionary<int, OrderedDictionary> boxes = Enumerable.Range(0, 256).ToDictionary(i => i, i => new OrderedDictionary());
        var lenses = input.Split(",");
        var labels = new List<string>();
        foreach (var l in lenses)
        {
            var opIdx = l.IndexOf('=') > 0 ? l.IndexOf('=') : l.IndexOf('-');
            string label = l[..opIdx];
            int box = CalcHash(label);
            if (l[opIdx] == '-')
            {
                boxes[box].Remove(label);
            }
            else
            {
                _ = int.TryParse(l[(opIdx + 1)..], out int focal);
                boxes[box][label] = focal;
                if (!labels.Contains(label)) labels.Add(label);
            }
        }

        return boxes.Sum(b => b.Value.Keys.Cast<string>().Select((l, idx) => (l, idx)).Zip(b.Value.Values.Cast<int>())
            .Sum(lens => (b.Key + 1) * (lens.First.idx + 1) * lens.Second));
    }

    private int CalcHash(string s)
    {
        return s.Aggregate(0, (acc, c) => acc = (acc + c) * 17 % 256);
    }
}