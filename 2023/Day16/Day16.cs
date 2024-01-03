using Grid = char[][];

namespace AdventOfCode2023;

public partial class Day16 : IDay
{
    public enum Dir { Up, Down, Left, Right }

    record Point(int X, int Y);
    record Beam(Point P, Dir Dir);

    public long Part1(string input)
    {
        var grid = input.Split(Environment.NewLine)
            .Select(l => l.ToCharArray())
            .ToArray();

        Beam startBeam = new(new(0, 0), Dir.Right);
        return NavigateGrid(grid, startBeam);
    }

    private int NavigateGrid(Grid grid, Beam startBeam)
    {
        HashSet<Beam> visitedBeams = [];
        var queue = new Queue<Beam>();
        queue.Enqueue(startBeam);

        while (queue.Count > 0)
        {
            var beam = queue.Dequeue();
            Point currentPoint = beam.P;
            Dir currentDir = beam.Dir;

            if (currentPoint.X < 0 || currentPoint.X >= grid.Length || currentPoint.Y < 0 || currentPoint.Y >= grid[0].Length)
                continue;

            if (visitedBeams.Contains(beam))
                continue;

            visitedBeams.Add(beam);

            char currentTile = grid[currentPoint.X][currentPoint.Y];
            if (currentTile == '.')
            {
                var nextBeam = GetNextByDir(currentPoint, currentDir);
                queue.Enqueue(nextBeam);
            }
            else
            {
                if (currentTile == '|')
                {
                    if (currentDir == Dir.Up || currentDir == Dir.Down)
                    {
                        var nextBeam = GetNextByDir(currentPoint, currentDir);
                        queue.Enqueue(nextBeam);
                    }
                    else
                    {
                        // Split
                        var nextBeamUp = GetNextByDir(currentPoint, Dir.Up);
                        var nextBeamDown = GetNextByDir(currentPoint, Dir.Down);
                        queue.Enqueue(nextBeamUp);
                        queue.Enqueue(nextBeamDown);
                    }
                }
                else if (currentTile == '-')
                {
                    if (currentDir == Dir.Left || currentDir == Dir.Right)
                    {
                        var nextBeam = GetNextByDir(currentPoint, currentDir);
                        queue.Enqueue(nextBeam);
                    }
                    else
                    {
                        // Split
                        var nextBeamLeft = GetNextByDir(currentPoint, Dir.Left);
                        var nextBeamRight = GetNextByDir(currentPoint, Dir.Right);
                        queue.Enqueue(nextBeamLeft);
                        queue.Enqueue(nextBeamRight);
                    }
                }
                else if (currentTile == '/')
                {
                    if (currentDir == Dir.Left)
                    {
                        var nextBeam = GetNextByDir(currentPoint, Dir.Down);
                        queue.Enqueue(nextBeam);
                    }
                    else if (currentDir == Dir.Right)
                    {
                        var nextBeam = GetNextByDir(currentPoint, Dir.Up);
                        queue.Enqueue(nextBeam);
                    }
                    else if (currentDir == Dir.Up)
                    {
                        var nextBeam = GetNextByDir(currentPoint, Dir.Right);
                        queue.Enqueue(nextBeam);
                    }
                    else if (currentDir == Dir.Down)
                    {
                        var nextBeam = GetNextByDir(currentPoint, Dir.Left);
                        queue.Enqueue(nextBeam);
                    }
                }
                else if (currentTile == '\\')
                {
                    if (currentDir == Dir.Left)
                    {
                        var nextBeam = GetNextByDir(currentPoint, Dir.Up);
                        queue.Enqueue(nextBeam);
                    }
                    else if (currentDir == Dir.Right)
                    {
                        var nextBeam = GetNextByDir(currentPoint, Dir.Down);
                        queue.Enqueue(nextBeam);
                    }
                    else if (currentDir == Dir.Up)
                    {
                        var nextBeam = GetNextByDir(currentPoint, Dir.Left);
                        queue.Enqueue(nextBeam);
                    }
                    else if (currentDir == Dir.Down)
                    {
                        var nextBeam = GetNextByDir(currentPoint, Dir.Right);
                        queue.Enqueue(nextBeam);
                    }
                }
            }
        }

        return visitedBeams.Select(b => b.P).Distinct().Count();
    }

    private Beam GetNextByDir(Point currentPoint, Dir nextDir)
    {
        Point nextBeam = nextDir switch
        {
            Dir.Up => new(currentPoint.X - 1, currentPoint.Y),
            Dir.Down => new(currentPoint.X + 1, currentPoint.Y),
            Dir.Left => new(currentPoint.X, currentPoint.Y - 1),
            Dir.Right => new(currentPoint.X, currentPoint.Y + 1),
            _ => throw new NotImplementedException()
        };

        return new(nextBeam, nextDir);
    }

    public long Part2(string input)
    {
        var grid = input.Split(Environment.NewLine)
            .Select(l => l.ToCharArray())
            .ToArray();

        return Enumerable.Range(0, grid.Length).SelectMany(r => new int[] {
            NavigateGrid(grid, new(new(r, 0), Dir.Right)),
            NavigateGrid(grid, new(new(r, grid[0].Length - 1), Dir.Left))
        }).Concat(Enumerable.Range(0, grid[0].Length).SelectMany(c => new int[] {
            NavigateGrid(grid, new(new(0, c), Dir.Down)),
            NavigateGrid(grid, new(new(grid.Length - 1, c), Dir.Up))
        })).Max();
    }
}