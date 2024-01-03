using Grid = char[][];

namespace AdventOfCode2023;

public partial class Day14 : IDay
{
    private Grid Parse(string input) => input.Split(Environment.NewLine).Select(l => l.ToCharArray()).ToArray();

    private Grid Tilt(Grid grid)
    {
        int numRows = grid.Length;
        int numCols = grid[0].Length;

        for (int c = 0; c < numCols; c++)
        {
            var emptyRow = 0;
            for (int r = 0; r < numRows; r++)
            {
                if (grid[r][c] == '#') emptyRow = r + 1;
                else if (grid[r][c] == 'O')
                {
                    grid[r][c] = '.';
                    grid[emptyRow++][c] = 'O';
                }
            }
        }

        return grid;
    }

    private Grid Rotate(Grid grid)
    {
        int numRows = grid.Length;
        int numCols = grid[0].Length;
        Grid rotated = new char[numCols][];
        for (int r = 0; r < numRows; r++)
        {
            rotated[r] = new char[numRows];
            for (int c = 0; c < numCols; c++)
            {
                rotated[r][c] = grid[numRows - c - 1][r];
            }
        }

        return rotated;
    }

    private int GetLoad(Grid grid)
    {
        return grid.Select((row, rowIdx) => row.Count(c => c == 'O') * (grid.Length - rowIdx)).Sum();
    }

    private Grid Cycle(Grid grid)
    {
        var history = new List<String>();

        int cycle = 1000000000;
        while (cycle > 0)
        {
            for (int i = 0; i < 4; i++)
                grid = Rotate(Tilt(grid));
            cycle--;

            string hGrid = String.Join(Environment.NewLine, grid.Select(r => new string(r)));
            var historyIdx = history.IndexOf(hGrid);
            if (historyIdx < 0)
                history.Add(hGrid);
            else
            {
                var loopLength = history.Count - historyIdx;
                var remainder = cycle % loopLength;
                return Parse(history[historyIdx + remainder]);
            }
        }
        return grid;
    }

    private void Print(Grid grid)
    {
        grid.ToList().ForEach(r => System.Console.WriteLine(new string(r)));
        System.Console.WriteLine();
    }

    public long Part1(string input)
    {
        return GetLoad(Tilt(Parse(input)));
    }

    public long Part2(string input)
    {
        return GetLoad(Cycle(Parse(input)));
    }
}