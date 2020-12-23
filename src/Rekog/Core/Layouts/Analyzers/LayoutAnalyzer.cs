using Rekog.Core.Corpora;
using System.CommandLine;

namespace Rekog.Core.Layouts.Analyzers
{
    public abstract class LayoutAnalyzer
    {
        protected LayoutAnalyzer(string description)
        {
            Description = description;
        }

        public string Description { get; }

        public abstract void Analyze(CorpusAnalysis corpusAnalysis, Layout layout);

        public abstract void Print(IConsole console);
    }
}
