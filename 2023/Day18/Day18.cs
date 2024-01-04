using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public partial class Day18 : IDay
{
    record Point(int X, int Y)
    {
        public Point Add(Point p) => new(X + p.X, Y + p.Y);
    }

    private readonly Dictionary<string, Point> Moves = new() {
        { "U", new(-1, 0) },
        { "D", new(1, 0) },
        { "L", new(0, -1) },
        { "R", new(0, 1) }
    };

    public long Part1(string input)
    {
        var points = new List<Point>() { new(0, 0) };
        foreach (var line in input.Split(Environment.NewLine))
        {
            var parts = line.Split(" ");
            var moveDir = Moves[parts[0]];
            var length = int.Parse(parts[1]);
            for (int i = 0; i < length; i++)
                points.Add(points.Last().Add(moveDir));
        }

        return polygonArea(points) - (points.Count - 1) / 2 + 1 + (points.Count - 1);
    }

    public long Part2(string input)
    {
        var points = new List<Point>() { new(0, 0) };
        foreach (var line in input.Split(Environment.NewLine))
        {
            var hexValue = Regex.Match(line, @"\((.*)\)").Groups[1].Value; // #70c710
            var dirCode = hexValue.Last() switch
            {
                '0' => "R",
                '1' => "D",
                '2' => "L",
                '3' => "U",
                _ => throw new Exception()
            };
            var moveDir = Moves[dirCode];
            var length = int.Parse(hexValue[1..(hexValue.Length - 1)], System.Globalization.NumberStyles.HexNumber);
            for (int i = 0; i < length; i++)
                points.Add(points.Last().Add(moveDir));
        }

        return polygonArea(points) - (points.Count - 1) / 2 + 1 + (points.Count - 1);
    }

    private static long polygonArea(List<Point> points)
    {
        long area = 0;
        int n = points.Count;
        int j = n - 1;

        for (int i = 0; i < n; i++)
        {
            area += points[j].X * points[i].Y - points[j].Y * points[i].X;
            j = i;
        }

        return Math.Abs(area / 2);
    }
}