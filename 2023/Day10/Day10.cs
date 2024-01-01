using System.Security.Cryptography;

namespace AdventOfCode2023;

public partial class Day10 : IDay
{
    public enum Dir
    {
        Up,
        Down,
        Left,
        Right
    }

    // grid[X][Y] => X=row, Y=column
    public class Point
    {
        public int X { get; }
        public int Y { get; }
        public char Symbol { get; }

        public Point(int x, int y, char symbol)
        {
            X = x;
            Y = y;
            Symbol = symbol;
        }

        public Point? GetAdj(Dir dir, char[][] grid)
        {
            var newCoords = dir switch
            {
                Dir.Up => (X - 1, Y),
                Dir.Down => (X + 1, Y),
                Dir.Left => (X, Y - 1),
                Dir.Right => (X, Y + 1),
                _ => throw new NotImplementedException()
            };

            if (newCoords.Item1 >= 0 && newCoords.Item1 < grid.Length && newCoords.Item2 >= 0 && newCoords.Item2 < grid.Length)
                return new Point(newCoords.Item1, newCoords.Item2, grid[newCoords.Item1][newCoords.Item2]);
            return null;
        }

        public IEnumerable<Dir> GetValidDirs()
        {
            return Symbol switch
            {
                '-' => [Dir.Left, Dir.Right],
                '|' => [Dir.Up, Dir.Down],
                '7' => [Dir.Left, Dir.Down],
                'J' => [Dir.Left, Dir.Up],
                'L' => [Dir.Up, Dir.Right],
                'F' => [Dir.Down, Dir.Right],
                'S' => [Dir.Up, Dir.Down, Dir.Left, Dir.Right],
                _ => []
            };
        }

        public bool IsPipe() => new char[] { '-', '|', '7', 'J', 'L', 'F', 'S' }.Contains(Symbol);

        public override bool Equals(object? obj)
        {
            if (obj is Point point) return X == point?.X && Y == point?.Y;
            return false;
        }

        public override int GetHashCode() => this.GetHashCode();
    }

    public long Part1(string input)
    {
        var grid = input.Split(Environment.NewLine)
            .Select(l => l.ToCharArray())
            .ToArray();

        var totalPoints = NavigatePipes(grid);
        return totalPoints.Count / 2;
    }

    private List<Point> NavigatePipes(char[][] grid)
    {
        // Find start point
        var startTotalIdx = grid.SelectMany(p => p).ToList().IndexOf('S');
        Point startPoint = new((int)Math.Floor((decimal)startTotalIdx / grid.Length), startTotalIdx % grid.Length, 'S');

        // Navigate
        var loopCompleted = false;
        List<Point> steps = [startPoint];
        Point currentPoint = startPoint;
        Point prevPoint = currentPoint;
        while (!loopCompleted)
        {
            foreach (Dir dir in currentPoint.GetValidDirs())
            {
                var adjPoint = currentPoint.GetAdj(dir, grid);
                if (adjPoint != null && !adjPoint.Equals(prevPoint) && adjPoint.IsPipe())
                {
                    prevPoint = currentPoint;
                    currentPoint = adjPoint;
                    break;
                }
            }

            steps.Add(currentPoint);
            if (currentPoint.Equals(startPoint)) loopCompleted = true;
        }

        return steps;
    }

    public long Part2(string input)
    {
        var grid = input.Split(Environment.NewLine)
            .Select(l => l.ToCharArray())
            .ToArray();

        var totalPoints = NavigatePipes(grid);
        return totalPoints.Count / 2;
    }
}