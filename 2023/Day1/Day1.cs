namespace AdventOfCode2023;

public partial class Day1 : IDay
{
    private List<string> lettersDigits =>
        ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

    public long Part1(string input)
    {
        return input
            .Split(Environment.NewLine)
            .Sum(line => int.Parse($"{line.First(char.IsNumber)}{line.Last(char.IsNumber)}"));
    }

    public long Part2(string input)
    {
        return input
            .Split(Environment.NewLine)
            .Select(line =>
            {
                int lineIdx = 0;
                return line.Aggregate<char, string>("", (acc, c) =>
                {
                    var newAcc = acc;
                    if (char.IsNumber(c))
                    {
                        newAcc += c;
                    }
                    else
                    {
                        var foundLetterDigit = lettersDigits.Where(ld => (lineIdx + ld.Length <= line.Length) && line.Substring(lineIdx, ld.Length) == ld).FirstOrDefault();
                        if (foundLetterDigit != null)
                        {
                            newAcc += (char)(lettersDigits.IndexOf(foundLetterDigit) + 49);
                        }
                    }

                    lineIdx++;
                    return newAcc;
                });
            })
            .Sum(line => int.Parse($"{line.First(char.IsNumber)}{line.Last(char.IsNumber)}"));
    }
}
