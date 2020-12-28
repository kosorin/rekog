namespace Rekog.Core.Layouts.Analyzers
{
    internal interface IUnigramAnalyzer : IOccurrenceAnalyzer
    {
        void Analyze(Key key, ulong count);
    }
}