namespace Rekog.Core.Layouts.Analyzers
{
    internal class RowFrequencyAnalyzer : UnigramAnalyzer<int>
    {
        public RowFrequencyAnalyzer()
            : base("Row frequencies")
        {
        }

        protected override bool TryAnalyze(Key key, out LayoutNgramAnalysis<int> result)
        {
            result = new LayoutNgramAnalysis<int>(key.Row);
            return true;
        }
    }
}
