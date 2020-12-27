using Rekog.Extensions;
using System.CommandLine;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    public abstract class OccurrenceLayoutAnalyzer<T> : LayoutAnalyzer
        where T : notnull
    {
        protected OccurrenceLayoutAnalyzer(string description) : base(description)
        {
        }

        protected OccurrenceCollection<T> Occurrences { get; } = new();

        public override void Print(IConsole console)
        {
            console.Out.WriteLine($"{Description}:");
            foreach (var item in new OccurrenceAnalysis<T>(Occurrences).OrderByDescending(x => x.Percentage))
            {
                console.Out.WriteLine($"{item.Percentage,10:P3}  {item.Value}");
            }
        }
    }

    public abstract class OccurrenceLayoutAnalyzer : OccurrenceLayoutAnalyzer<bool>
    {
        protected OccurrenceLayoutAnalyzer(string description) : base(description)
        {
        }

        public override void Print(IConsole console)
        {
            var value = new OccurrenceAnalysis<bool>(Occurrences).TryGet(true, out var alternation) ? alternation.Percentage : 0;
            console.Out.WriteLine($"{Description}: {value,10:P3}");
        }
    }
}
