namespace Rekog.Core.Layouts.Analyzers
{
    internal interface INgramAnalyzer : IAnalyzer
    {
        int Size { get; }

        void Analyze(Occurrence<Key[]> ngram);

        void AnalyzeNull(ulong count);
    }
}