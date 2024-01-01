namespace AdventOfCode2023;

class Program
{
    static void Main(string[] args)
    {
        var days = typeof(IDay).Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IDay)))
            .Select(t => (IDay)Activator.CreateInstance(t)!)
            .OrderBy(d => int.Parse(d.GetType().Name.Replace("Day", "")))
            .Where(d =>
            {
                if (args.Length == 0) return true;
                var dayNumber = d.GetType().Name.Replace("Day", "");
                return args.Contains(dayNumber);
            });

        foreach (var day in days)
        {
            Console.WriteLine($"{day.GetType().Name}");

            var inputFileName = args.Contains("sample") ? "input_sample.txt" : "input.txt";
            var input = File.ReadAllText($"{day.GetType().Name}/{inputFileName}");
            var part1 = day.Part1(input);
            Console.WriteLine($"    Part One: {part1}");

            var part2 = day.Part2(input);
            Console.WriteLine($"    Part Two: {part2}");
        }
    }
}
