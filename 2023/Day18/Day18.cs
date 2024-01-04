using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public partial class Day18 : IDay
{
    record Point(int X, int Y);

    public long Part1(string input)
    {
        Point p = new(0, 0);
        var points = new List<Point>();
        foreach (var line in input.Split(Environment.NewLine))
        {
            var parts = line.Split(" ");
            Point moveDir = parts[0] switch
            {
                "U" => new(-1, 0),
                "D" => new(1, 0),
                "L" => new(0, -1),
                "R" => new(0, 1),
                _ => throw new Exception()
            };
            var length = int.Parse(parts[1]);
            for (int i = 0; i < length; i++)
            {
                p = new(p.X + moveDir.X, p.Y + moveDir.Y);
                points.Add(p);
            }
        }

        return CalculateArea(points) - (points.Count) / 2 + 1 + (points.Count);
    }

    public long Part2(string input)
    {
        Point p = new(0, 0);
        var points = new List<Point>();
        var borderLength = 0;
        foreach (var line in input.Split(Environment.NewLine))
        {
            var hexValue = Regex.Match(line, @"\((.*)\)").Groups[1].Value; // #70c710
            var length = Convert.ToInt32(hexValue[1..(hexValue.Length - 1)], 16);
            Point moveDir = hexValue.Last() switch
            {
                '0' => new(0, length),
                '1' => new(length, 0),
                '2' => new(0, -length),
                '3' => new(-length, 0),
                _ => throw new Exception()
            };
            borderLength += length;
            p = new(p.X + moveDir.X, p.Y + moveDir.Y);
            points.Add(p);
        }

        return CalculateArea(points) - borderLength / 2 + 1 + borderLength;
    }

    // Shoelace formula
    long CalculateArea(List<Point> vertices)
    {
        int numVertices = vertices.Count;
        long area = 0;

        for (int i = 0; i < numVertices; i++)
        {
            int j = (i + 1) % numVertices;
            area += (long)vertices[i].X * vertices[j].Y - (long)vertices[j].X * vertices[i].Y;
        }

        return Math.Abs(area / 2);
    }
}