namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class Analyzer : IAnalyzer
    {
        protected Analyzer(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
