using System.Collections.Immutable;

namespace AdventOfCode2023;

public partial class Day7 : IDay
{
    class Hand : IComparable<Hand>
    {
        private static string cardOrder = "23456789TJQKA";
        private static string cardOrderWithJoker = "J23456789TQKA";

        public string Cards { get; }
        public int Bid { get; }

        private bool useJokers = false;

        public Hand(string line, bool useJokers = false)
        {
            var lineParts = line.Split(" ");
            Cards = lineParts[0];
            Bid = int.Parse(lineParts[1]);
            this.useJokers = useJokers;
        }

        public int HandType => useJokers ? CalculateTypeWithJoker() : CalculateType();

        public int CompareTo(Hand? other)
        {
            var orderToCheck = useJokers ? cardOrderWithJoker : cardOrder;
            if (other is null) return 1;
            return Cards.Zip(other.Cards, (a, b) => orderToCheck.IndexOf(a).CompareTo(orderToCheck.IndexOf(b))).FirstOrDefault(c => c != 0, 0);
        }

        private int CalculateType()
        {
            var groupedOrderedCards = Cards.GroupBy(c => c).ToImmutableDictionary(g => g.Key, g => g.Count())
                .OrderByDescending(kvp => kvp.Value).ToList();
            return groupedOrderedCards[0].Value switch
            {
                5 => 7,
                4 => 6,
                3 => groupedOrderedCards[1].Value == 2 ? 5 : 4,
                2 => groupedOrderedCards[1].Value == 2 ? 2 : 1,
                _ => 0
            };
        }

        private int CalculateTypeWithJoker()
        {
            if (Cards.All(c => c == 'J')) return 7;

            var groupedCards = Cards.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            var maxKeyNotJoker = groupedCards.Where(kvp => kvp.Key != 'J').MaxBy(kvp => kvp.Value).Key;

            // Add jokers count to the card (not joker) with most count and remove jokers
            foreach (var kvp in groupedCards)
            {
                if (kvp.Key == 'J')
                {
                    groupedCards[maxKeyNotJoker] += kvp.Value;
                    groupedCards.Remove(kvp.Key);
                }
            }

            var groupedOrderedCards = groupedCards.OrderByDescending(kvp => kvp.Value).ToList();
            return groupedOrderedCards[0].Value switch
            {
                5 => 7,
                4 => 6,
                3 => groupedOrderedCards[1].Value == 2 ? 5 : 4,
                2 => groupedOrderedCards[1].Value == 2 ? 2 : 1,
                _ => 0
            };
        }
    }

    public long Part1(string input)
    {
        return input.Split(Environment.NewLine)
            .Select(l => new Hand(l))
            .OrderBy(h => h.HandType)
            .ThenBy(h => h)
            .Select((h, idx) => h.Bid * (idx + 1))
            .Sum();
    }

    public long Part2(string input)
    {
        return input.Split(Environment.NewLine)
            .Select(l => new Hand(l, true))
            .OrderBy(h => h.HandType)
            .ThenBy(h => h)
            .Select((h, idx) => h.Bid * (idx + 1))
            .Sum();
    }
}