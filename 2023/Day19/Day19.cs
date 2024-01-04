using System.Diagnostics;
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
        private List<Rule> rules;
        public List<Rule> Rules => rules;

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

        public (string, RangedRating? included, RangedRating? excluded) Process(RangedRating r, Rule rule)
        {
            // Final value (mandatory workflow or acceptance)
            if (rule.InputValue is null) return (rule.Destination, r, null);

            (int minValue, int maxValue) inputs = rule.InputValue switch
            {
                "x" => (r.MinX, r.MaxX),
                "m" => (r.MinM, r.MaxM),
                "a" => (r.MinA, r.MaxA),
                "s" => (r.MinS, r.MaxS),
                _ => throw new Exception()
            };

            int compareValue = rule.CompareValue!.Value;

            // if rating is all excluded: go to next rule
            // if rating is all included: returns next and rating
            if (rule.Operand == ">")
            {
                if (inputs.maxValue <= compareValue) return (rule.Destination, null, r);
                else if (inputs.minValue > compareValue) return (rule.Destination, r, null);
            }
            else if (rule.Operand == "<")
            {
                if (inputs.minValue >= compareValue) return (rule.Destination, null, r);
                else if (inputs.maxValue < compareValue) return (rule.Destination, r, null);
            }

            // Here we should have only both included and excluded ranged
            var minInc = rule.Operand == ">" ? compareValue + 1 : inputs.minValue;
            var maxInc = rule.Operand == ">" ? inputs.maxValue : compareValue - 1;
            var minExc = rule.Operand == ">" ? inputs.minValue : compareValue;
            var maxExc = rule.Operand == ">" ? compareValue : inputs.maxValue;

            if (rule.InputValue == "x")
            {

                return (rule.Destination, r with { MinX = minInc, MaxX = maxInc }, r with { MinX = minExc, MaxX = maxExc });
            }
            else if (rule.InputValue == "a")
            {
                return (rule.Destination, r with { MinA = minInc, MaxA = maxInc }, r with { MinA = minExc, MaxA = maxExc });
            }
            else if (rule.InputValue == "m")
            {
                return (rule.Destination, r with { MinM = minInc, MaxM = maxInc }, r with { MinM = minExc, MaxM = maxExc });
            }
            else if (rule.InputValue == "s")
            {
                return (rule.Destination, r with { MinS = minInc, MaxS = maxInc }, r with { MinS = minExc, MaxS = maxExc });
            }
            else throw new Exception();
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
        var inputParts = input.Split(Environment.NewLine + Environment.NewLine);
        workflows = inputParts[0].Split(Environment.NewLine)
            .Select(l => l.Split(new string[] { "{", "}" }, StringSplitOptions.None))
            .ToDictionary(p => p[0], p => new Workflow(p[1]));

        var startWorkflow = workflows["in"];
        var startRatings = new RangedRating(1, 4000, 1, 4000, 1, 4000, 1, 4000);
        var queue = new Queue<(Workflow workflow, RangedRating)>();
        var accepted = new List<RangedRating>();
        queue.Enqueue((startWorkflow, startRatings));
        while (queue.Count > 0)
        {
            var (workflow, rating) = queue.Dequeue();
            foreach (Rule rule in workflow.Rules)
            {
                var (next, included, excluded) = workflow.Process(rating, rule);
                if (next == "A") accepted.Add(included!);
                else if (next != null && next != "R") queue.Enqueue((workflows[next], included!));

                if (excluded != null) rating = excluded;
                else break;
            }
        }

        return accepted.Sum(rr => (long)(rr.MaxX - rr.MinX + 1) * (rr.MaxA - rr.MinA + 1)
            * (rr.MaxM - rr.MinM + 1) * (rr.MaxS - rr.MinS + 1));
    }
}