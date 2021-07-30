namespace Rekog.Core.Layouts.Analyzers
{
    internal interface IAnalyzer
    {
        string Description { get; }

        LayoutAnalysisNode GetResult();
    }
}
