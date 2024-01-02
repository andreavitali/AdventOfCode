namespace AdventOfCode2023;

public partial class Day13 : IDay
{
    public long Part1(string input)
    {
        var patterns = input.Split(Environment.NewLine + Environment.NewLine).Select(p => p.Split(Environment.NewLine));
        return patterns.Sum(p => GetMirrors(p, 0));
    }

    private int GetMirrors(string[] pattern, int distance)
    {
        var rows = pattern.Select(r => Convert.ToInt32(r.Replace('.', '0').Replace('#', '1'), 2)).ToList();
        var cols = Enumerable.Range(0, pattern[0].Length).Select(cIdx =>
            Convert.ToInt32(String.Join("", pattern.Select(r => r[cIdx])).Replace('.', '0').Replace('#', '1'), 2)).ToList();

        return GetMirrorIndex(rows, distance, cols.Count) * 100 + GetMirrorIndex(cols, distance, rows.Count);
    }

    private int GetMirrorIndex(List<int> values, int distance, int byteLength) =>
        Enumerable.Range(1, values.Count - 1).FirstOrDefault(i =>
            values[..i].Reverse<int>().Zip(values[i..]).Sum(t => GetDistance(t.First, t.Second, byteLength)) == distance);

    private int GetDistance(int first, int second, int byteLength) =>
        Enumerable.Range(0, byteLength).Count(b => ((first >> b) & 1) != ((second >> b) & 1));

    public long Part2(string input)
    {
        var patterns = input.Split(Environment.NewLine + Environment.NewLine).Select(p => p.Split(Environment.NewLine));
        return patterns.Sum(p => GetMirrors(p, 1));
    }
}