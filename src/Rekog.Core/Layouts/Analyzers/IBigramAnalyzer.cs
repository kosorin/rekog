namespace Rekog.Core.Layouts.Analyzers
{
    internal interface IBigramAnalyzer : IOccurrenceAnalyzer
    {
        void Analyze(Key firstKey, Key secondKey, ulong count);
    }
}