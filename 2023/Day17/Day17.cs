using Grid = System.Collections.Generic.Dictionary<(int, int), int>;

namespace AdventOfCode2023;

public partial class Day17 : IDay
{
    public enum Dir { Up, Down, Left, Right }
    record Point(int X, int Y);
    record Crucible(int X, int Y, Dir Direction, int StraightCount);

    public long Part1(string input)
    {
        return MinHeatingLoss(ParseInput(input), 0, 3);
    }

    public long Part2(string input)
    {
        return MinHeatingLoss(ParseInput(input), 4, 10);
    }

    private Grid ParseInput(string input)
    {
        return input.Split(Environment.NewLine)
            .SelectMany((r, rowIdx) => r.Select((c, colIdx) => ((rowIdx, colIdx), c - '0')))
            .ToDictionary(p => p.Item1, p => p.Item2);
    }

    private int MinHeatingLoss(Grid grid, int minStraight, int maxStraight)
    {
        HashSet<Crucible> visitedCrucibles = [];
        int rowCount = grid.Keys.Max(k => k.Item1) + 1;
        int colCount = grid.Keys.Max(k => k.Item2) + 1;

        var queue = new PriorityQueue<Crucible, int>();
        queue.Enqueue(new(0, 0, Dir.Right, 1), 0);
        queue.Enqueue(new(0, 0, Dir.Down, 1), 0);
        while (queue.TryDequeue(out var cr, out int totalHeat))
        {
            if (visitedCrucibles.Contains(cr))
                continue;

            if (cr.X == rowCount - 1 && cr.Y == colCount - 1)
                return totalHeat;

            foreach (var next in NextMoves(cr, minStraight, maxStraight))
            {
                if (grid.ContainsKey((next.X, next.Y)))
                {
                    visitedCrucibles.Add(cr);
                    queue.Enqueue(next, totalHeat + grid[(next.X, next.Y)]);
                }
            }
        }

        return 0;
    }

    private IEnumerable<Crucible> NextMoves(Crucible cr, int minStraight, int maxStraight)
    {
        if (cr.StraightCount < maxStraight)
        {
            // can continue in that direction
            yield return new Crucible(
                cr.Direction == Dir.Left || cr.Direction == Dir.Right ? cr.X : (cr.Direction == Dir.Up ? cr.X - 1 : cr.X + 1),
                cr.Direction == Dir.Up || cr.Direction == Dir.Down ? cr.Y : (cr.Direction == Dir.Left ? cr.Y - 1 : cr.Y + 1),
                cr.Direction,
                cr.StraightCount + 1
            );
        }

        if (cr.StraightCount >= minStraight)
        {
            if (cr.Direction == Dir.Up || cr.Direction == Dir.Down)
            {
                // go only left or right
                yield return new Crucible(cr.X, cr.Y - 1, Dir.Left, 1);
                yield return new Crucible(cr.X, cr.Y + 1, Dir.Right, 1);
            }
            else
            {
                // go only up or down
                yield return new Crucible(cr.X - 1, cr.Y, Dir.Up, 1);
                yield return new Crucible(cr.X + 1, cr.Y, Dir.Down, 1);
            }
        }
    }
}
