using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public partial class Day19 : IDay
{
    record Rating(int X, int M, int A, int S, bool Accepted = false);
    record RangedRating(int MinX, int MaxX, int MinM, int MaxM, int MinA, int MaxA, int MinS, int MaxS);
    record Rule(string? InputValue, string? Operand, int? CompareValue, string Destination);

    private Dictionary<string, Workflow> workflows = [];
    private IEnumerable<Rating> ratings = [];

    class Workflow
    {
        List<Rule> rules;

        public Workflow(string workflow)
        {
            this.rules = workflow.Split(",").Select(r =>
            {
                var match = Regex.Match(r, @"^([asmx])([><])(\d+):(\w+)");
                if (match.Success)
                {
                    return new Rule(match.Groups[1].Value, match.Groups[2].Value,
                        int.Parse(match.Groups[3].Value), match.Groups[4].Value);
                }
                else
                {
                    return new Rule(null, null, null, r);
                }
            }).ToList();
        }

        public string Process(Rating r)
        {
            foreach (Rule rule in rules)
            {
                if (rule.InputValue is null)
                    return rule.Destination;

                _ = int.TryParse(r.GetType().GetProperty(rule.InputValue.ToUpper())?.GetValue(r)?.ToString(), out int input);
                if ((rule.Operand == ">" && input > rule.CompareValue) || rule.Operand == "<" && input < rule.CompareValue)
                    return rule.Destination;
            }

            return "R";
        }

        public (string, RangedRating? included, RangedRating? excluded) Process(RangedRating r)
        {
            foreach (Rule rule in rules)
            {
                if (rule.InputValue is null)
                    return (rule.Destination, r, null);

                var inputs = rule.InputValue switch
                {
                    "x" => (r.MinX, r.MaxX),
                    "m" => (r.MinM, r.MaxM),
                    "a" => (r.MinA, r.MaxA),
                    "s" => (r.MinS, r.MaxS),
                    _ => throw new Exception()
                };

                int minIncluded = -1, maxIncluded = -1, minExcluded = -1, maxExcluded = -1;
                int compareValue = rule.CompareValue!.Value;

                if (rule.Operand == ">")
                {
                    // both included and excluded
                    if (inputs.Item1 <= compareValue && inputs.Item2 > compareValue)
                    {
                        minIncluded = compareValue + 1;
                        maxIncluded = inputs.Item2;
                        minExcluded = inputs.Item1;
                        maxExcluded = compareValue;
                    }
                    else if (inputs.Item2 <= compareValue) // all excluded
                    {
                        minExcluded = inputs.Item1;
                        maxExcluded = inputs.Item2;
                    }
                    else if (inputs.Item1 > compareValue) // all included
                    {
                        minIncluded = inputs.Item1;
                        maxIncluded = inputs.Item2;
                    }
                    else throw new Exception("Is this for real??");
                }
                else if (rule.Operand == "<")
                {
                    // both included and excluded
                    if (inputs.Item1 < compareValue && inputs.Item2 >= compareValue)
                    {
                        minIncluded = inputs.Item1;
                        maxIncluded = compareValue - 1;
                        minExcluded = compareValue;
                        maxExcluded = inputs.Item2;
                    }
                    else if (inputs.Item1 >= compareValue) // all excluded
                    {
                        minExcluded = inputs.Item1;
                        maxExcluded = inputs.Item2;
                    }
                    else if (inputs.Item2 < compareValue) // all included
                    {
                        minIncluded = inputs.Item1;
                        maxIncluded = inputs.Item2;
                    }
                    else throw new Exception("Is this for real??");
                }

                if (minIncluded == -1 && minExcluded >= 0)
                {
                    return (rule.Destination, null, r);
                }
                else if (minExcluded == -1 && minIncluded >= 0)
                {
                    return (rule.Destination, r, null);
                }
                else
                {
                    if (rule.InputValue == "x")
                    {
                        return (rule.Destination, new(minIncluded, maxIncluded, r.MinM, r.MaxM, r.MinA, r.MaxA, r.MinS, r.MaxS),
                            new(minExcluded, maxExcluded, r.MinM, r.MaxM, r.MinA, r.MaxA, r.MinS, r.MaxS));
                    }
                    else if (rule.InputValue == "a")
                    {
                        return (rule.Destination, new(r.MinX, r.MaxX, r.MinM, r.MaxM, minIncluded, maxIncluded, r.MinS, r.MaxS),
                            new(r.MinX, r.MaxX, r.MinM, r.MaxM, minExcluded, maxExcluded, r.MinS, r.MaxS));
                    }
                    else if (rule.InputValue == "m")
                    {
                        return (rule.Destination, new(r.MinX, r.MaxX, minIncluded, maxIncluded, r.MinA, r.MaxA, r.MinS, r.MaxS),
                            new(r.MinX, r.MaxX, minExcluded, maxExcluded, r.MinA, r.MaxA, r.MinS, r.MaxS));
                    }
                    else if (rule.InputValue == "s")
                    {
                        return (rule.Destination, new(r.MinX, r.MaxX, r.MinM, r.MaxM, r.MinA, r.MaxA, minIncluded, maxIncluded),
                            new(r.MinX, r.MaxX, r.MinM, r.MaxM, r.MinA, r.MaxA, minExcluded, maxExcluded));
                    }
                }
            }

            return ("R", null, null);
        }
    }

    private Rating ConsumeRecursive(Rating r, Workflow w)
    {
        var processResult = w.Process(r);
        if (processResult == "A")
            return r with { Accepted = true };
        else if (processResult == "R")
            return r with { Accepted = false };
        else
            return ConsumeRecursive(r, workflows[processResult]);
    }

    public long Part1(string input)
    {
        var inputParts = input.Split(Environment.NewLine + Environment.NewLine);
        workflows = inputParts[0].Split(Environment.NewLine)
            .Select(l => l.Split(new string[] { "{", "}" }, StringSplitOptions.None))
            .ToDictionary(p => p[0], p => new Workflow(p[1]));

        ratings = inputParts[1].Split(Environment.NewLine).Select(l =>
        {
            var matches = Regex.Matches(l, @"[xmas]=(\d+)");
            return new Rating(int.Parse(matches[0].Groups[1].Value), int.Parse(matches[1].Groups[1].Value),
                int.Parse(matches[2].Groups[1].Value), int.Parse(matches[3].Groups[1].Value));
        });

        var startWorkflow = workflows["in"];
        return ratings.Select(r => ConsumeRecursive(r, startWorkflow)).Where(r => r.Accepted).Sum(r => r.X + r.A + r.M + r.S);
    }

    public long Part2(string input)
    {
        // var inputParts = input.Split(Environment.NewLine + Environment.NewLine);
        // workflows = inputParts[0].Split(Environment.NewLine)
        //     .Select(l => l.Split(new string[] { "{", "}" }, StringSplitOptions.None))
        //     .ToDictionary(p => p[0], p => new Workflow(p[1]));

        // var startWorkflow = workflows["in"];
        // var startRatings = new RangedRating(0, 4000, 0, 4000, 0, 4000, 0, 4000);
        // var queue = new Queue<(Workflow workflow, RangedRating)>();
        // var accepted = new List<RangedRating>();
        // queue.Enqueue((startWorkflow, startRatings));
        // while (queue.Count > 0)
        // {
        //     var (workflow, rating) = queue.Dequeue();
        //     var (next, included, excluded) = workflow.Process(rating);

        //     if (next == "A") accepted.Add(included!);
        //     else if (next != null && next != "R") queue.Enqueue((workflows[next], included!));

        //     if (excluded != null) queue.Enqueue((workflow, excluded!));
        // }
        return 0;
    }
}