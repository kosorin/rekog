namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class OccurrenceAnalyzer<T> : Analyzer, IOccurrenceAnalyzer
        where T : notnull
    {
        protected OccurrenceAnalyzer(string description) : base(description)
        {
        }

        protected OccurrenceCollection<T> Occurrences { get; } = new();

        public void Skip(ulong count)
        {
            Occurrences.AddNull(count);
        }

        //public override void Print(IConsole console)
        //{
        //    console.Out.WriteLine($"{Description}:");
        //    foreach (var item in new OccurrenceAnalysis<T>(Occurrences).OrderByDescending(x => x.Percentage))
        //    {
        //        console.Out.WriteLine($"{item.Percentage,10:P3}  {item.Value}");
        //    }
        //}

        //public override void Print(IConsole console)
        //{
        //    var value = new OccurrenceAnalysis<bool>(Occurrences).TryGet(true, out var alternation) ? alternation.Percentage : 0;
        //    console.Out.WriteLine($"{Description}: {value,10:P3}");
        //}
    }
}
