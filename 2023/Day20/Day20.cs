namespace AdventOfCode2023;

public partial class Day20 : IDay
{
    abstract class Module
    {
        public string Name { get; private set; } = "";
        public IEnumerable<string> Destinations { get; private set; } = [];

        public abstract bool? Receive(bool pulse, string? input);

        public static Module Factory(string line)
        {
            var parts = line.Split("->", StringSplitOptions.TrimEntries);
            Module module = parts[0] == "broadcaster" ? new Broadcast()
            : parts[0].StartsWith('&') ? new Conjuntion()
            : parts[0].StartsWith('%') ? new FlipFlop()
            : throw new Exception("wrong module name " + parts[0]);

            module.Destinations = parts[1].Split(",", StringSplitOptions.TrimEntries);
            module.Name = parts[0].TrimStart(['%', '&']);

            return module;
        }
    }

    class Broadcast : Module
    {
        public override bool? Receive(bool pulse, string? input)
        {
            return pulse;
        }
    }

    class FlipFlop : Module
    {
        private bool state = false;

        public override bool? Receive(bool pulse, string? input)
        {
            if (pulse) return null;

            state = !state;
            return state;
        }
    }

    class Conjuntion : Module
    {
        Dictionary<string, bool> inputsState = [];

        public void AddInputState(string inputModuleName)
        {
            inputsState.Add(inputModuleName, false);
        }

        public override bool? Receive(bool pulse, string? input)
        {
            inputsState[input!] = pulse;
            return !inputsState.Values.All(s => s);
        }

        public bool AllState(bool state) => inputsState.Keys.All(k => state);
    }

    public long Part1(string input)
    {
        var modules = input.Split(Environment.NewLine).Select(Module.Factory).ToDictionary(m => m.Name, m => m);

        // Set conjuntion inputs
        foreach (Conjuntion c in modules.Values.OfType<Conjuntion>())
        {
            foreach (Module m in modules.Values.Where(m => m.GetType() != typeof(Conjuntion) && m.Destinations.Contains(c.Name)))
            {
                c.AddInputState(m.Name);
            }
        }

        int lowPulses = 0;
        int highPulses = 0;

        // Press button 1000 times
        for (int i = 0; i < 1000; i++)
        {
            // src, dest, pulse
            var queue = new Queue<(string, string, bool)>();
            queue.Enqueue(("button", "broadcaster", false));

            while (queue.Count > 0)
            {
                var (src, dest, pulse) = queue.Dequeue();

                if (pulse) highPulses++;
                else lowPulses++;

                if (!modules.ContainsKey(dest)) continue;

                var destModule = modules[dest];
                var result = destModule.Receive(pulse, src);
                if (result.HasValue)
                {
                    foreach (var output in destModule.Destinations)
                        queue.Enqueue((dest, output, result.Value));
                }
            }
        }

        return lowPulses * highPulses;
    }

    public long Part2(string input)
    {
        var modules = input.Split(Environment.NewLine).Select(Module.Factory).ToDictionary(m => m.Name, m => m);

        // Set conjuntion inputs
        var conjuntions = modules.Values.OfType<Conjuntion>();
        foreach (Conjuntion c in conjuntions)
        {
            foreach (Module m in modules.Values.Where(m => m.GetType() != typeof(Conjuntion) && m.Destinations.Contains(c.Name)))
            {
                c.AddInputState(m.Name);
            }
        }

        // Find final conjuntion
        var finalConj = conjuntions.First(c => c.Destinations.Any(d => d == "rx"));

        // Find semi-final conjuntions
        var sfConj = conjuntions.Where(c => c.Destinations.ToList().Contains(finalConj.Name));

        var counts = conjuntions.Where(c => c.Destinations.ToList().Contains(finalConj.Name)).ToDictionary(c => c.Name, _ => 0);
        int buttonPress = 0;

        while (true)
        {
            buttonPress++;

            var queue = new Queue<(string, string, bool)>();
            queue.Enqueue(("button", "broadcaster", false));

            while (queue.Count > 0)
            {
                var (src, dest, pulse) = queue.Dequeue();

                if (!modules.ContainsKey(dest)) continue;

                var destModule = modules[dest];
                var result = destModule.Receive(pulse, src);

                if (destModule == finalConj && pulse && counts[src] == 0)
                    counts[src] = buttonPress;

                if (result.HasValue)
                {
                    foreach (var output in destModule.Destinations)
                        queue.Enqueue((dest, output, result.Value));
                }
            }

            if (counts.Values.All(v => v > 0))
                break;
        }

        return counts.Values.Select(v => (long)v).Aggregate((a, b) => a * b / GCD(a, b));
    }

    static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}