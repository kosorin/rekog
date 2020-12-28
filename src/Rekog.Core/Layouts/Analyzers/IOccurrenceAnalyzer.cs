namespace Rekog.Core.Layouts.Analyzers
{
    internal interface IOccurrenceAnalyzer : IAnalyzer
    {
        void Skip(ulong count);
    }
}