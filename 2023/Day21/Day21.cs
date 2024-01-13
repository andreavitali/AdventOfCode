namespace AdventOfCode2023;

public partial class Day21 : IDay
{
    public enum Dir { Up, Down, Left, Right }
    record Point(long X, long Y);

    private IEnumerable<Point> ParseInput(string input)
    {
        int rowIdx = 0;
        foreach (string row in input.Split(Environment.NewLine))
        {
            var colIdx = 0;
            foreach (var col in row)
            {
                if (col != '#') yield return new Point(rowIdx, colIdx);
                colIdx++;
            }
            rowIdx++;
        }
    }

    long Walk(HashSet<Point> map, int numOfSteps = 64)
    {
        var mapSize = Convert.ToInt32(map.MaxBy(p => p.Y)?.Y / 2);
        var startPoint = new Point(mapSize, mapSize);
        var positions = new HashSet<Point> { startPoint };
        for (int i = 0; i < numOfSteps; i++)
        {
            var temp = new HashSet<Point>();
            foreach (Point point in positions)
            {
                foreach (Dir dir in Enum.GetValues(typeof(Dir)))
                {
                    Point? newPoint = dir switch
                    {
                        Dir.Up => new(point.X - 1, point.Y),
                        Dir.Down => new(point.X + 1, point.Y),
                        Dir.Left => new(point.X, point.Y - 1),
                        Dir.Right => new(point.X, point.Y + 1),
                        _ => null
                    };

                    if (newPoint != null)
                    {
                        Point transposedPoint = new(Mod(newPoint.X, mapSize), Mod(newPoint.Y, mapSize));
                        if (map.Contains(transposedPoint)) temp.Add(newPoint);
                    }
                }
            }

            positions = temp;
        }
        return positions.Count;
    }

    long Mod(long n, int m) => ((n % m) + m) % m;

    public long Part1(string input)
    {
        var map = ParseInput(input).ToHashSet();
        return Walk(map, 64);
    }

    public long Part2(string input)
    {
        var map = ParseInput(input).ToHashSet();
        return Walk(map, 100);
    }
}