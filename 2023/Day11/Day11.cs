using System.Drawing;

namespace AdventOfCode2023;

public partial class Day11 : IDay
{
    class Galaxy(long x, long y)
    {
        public long X { get; set; } = x;
        public long Y { get; set; } = y;
    }

    public long Part1(string input)
    {
        var galaxies = input.Split(Environment.NewLine)
            .SelectMany((line, y) => line.Select((c, x) => (x, y, c)))
            .Where(p => p.c == '#')
            .Select(p => new Galaxy(p.x, p.y))
            .ToList();

        return Calculate(galaxies, 2);
    }

    public long Part2(string input)
    {
        var galaxies = input.Split(Environment.NewLine)
            .SelectMany((line, y) => line.Select((c, x) => (x, y, c)))
            .Where(p => p.c == '#')
            .Select(p => new Galaxy(p.x, p.y))
            .ToList();

        return Calculate(galaxies, 1000000);
    }

    private long Calculate(List<Galaxy> galaxies, long expandFactor)
    {
        // Expand rows
        var minY = galaxies.Min(g => g.Y);
        var maxY = galaxies.Max(g => g.Y);
        for (long y = maxY; y > minY; y--)
        {
            // y is an empty row
            if (!galaxies.Any(g => g.Y == y))
            {
                foreach (var g in galaxies.Where(g => g.Y > y))
                {
                    g.Y += expandFactor - 1;
                };
            }
        }

        // Expand columns
        var minX = galaxies.Min(g => g.X);
        var maxX = galaxies.Max(g => g.X);
        for (long x = maxX; x > minX; x--)
        {
            // y is an empty row
            if (!galaxies.Any(g => g.X == x))
            {
                foreach (var g in galaxies.Where(g => g.X > x))
                {
                    g.X += expandFactor - 1;
                };
            }
        }

        // Calculate manhattan distance
        long distance = 0;
        foreach (var g in galaxies)
        {
            foreach (var g2 in galaxies.Where(gg => gg != g))
            {
                distance += ManhattanDistance(g, g2);
            }
        }

        return distance / 2;
    }

    private long ManhattanDistance(Galaxy p1, Galaxy p2)
    {
        return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    }
}