namespace Rekog.Core.Layouts.Analyzers
{
    internal interface INgramAnalyzer : IAnalyzer
    {
        int Size { get; }

        void Analyze(Occurrence<LayoutNgram> ngram);

        void AnalyzeNull(ulong count);
    }
}