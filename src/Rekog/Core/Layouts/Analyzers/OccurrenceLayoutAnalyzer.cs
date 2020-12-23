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
}
