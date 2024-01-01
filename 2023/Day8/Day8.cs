namespace AdventOfCode2023;

public partial class Day8 : IDay
{
    private static readonly char[] instructionSeparator = [' ', '=', '(', ')', ','];

    private List<char>? directions;
    private IDictionary<string, (string, string)>? instructions;

    public long Part1(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        this.directions = lines[0].Select(d => d).ToList();
        this.instructions = lines.Skip(1)
            .Select(l => l.Split(instructionSeparator, StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(i => i[0], i => (i[1], i[2]));

        var steps = 0;
        var currentStep = "AAA";
        while (currentStep != "ZZZ")
        {
            var currentDir = directions[steps % directions.Count];
            currentStep = currentDir == 'L' ? instructions[currentStep].Item1 : instructions[currentStep].Item2;
            steps++;
        }

        return steps;
    }

    public long Part2(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var directions = lines[0].Select(d => d).ToList().ToArray();
        var instructions = lines.Skip(1)
            .Select(l => l.Split(instructionSeparator, StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(i => i[0], i => (i[1], i[2]));

        return instructions.Keys.Where(k => k.EndsWith('A'))
            .Select(s =>
            {
                var steps = 0L;
                var currentStep = s;
                while (!currentStep.EndsWith('Z'))
                {
                    var currentDir = directions[steps % directions.Length];
                    currentStep = currentDir == 'L' ? instructions[currentStep].Item1 : instructions[currentStep].Item2;
                    steps++;
                }
                return steps;
            })
            .Aggregate((acc, n) =>
            {
                long num1, num2;
                if (acc > n)
                {
                    num1 = acc;
                    num2 = n;
                }
                else
                {
                    num1 = n;
                    num2 = acc;
                }

                for (int i = 1; i <= num2; i++)
                {
                    if ((num1 * i) % num2 == 0)
                    {
                        return i * num1;
                    }
                }
                return num2;
            });
    }
}