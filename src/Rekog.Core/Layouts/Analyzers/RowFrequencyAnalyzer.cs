namespace Rekog.Core.Layouts.Analyzers
{
    internal class RowFrequencyAnalyzer : UnigramAnalyzer<int>
    {
        public RowFrequencyAnalyzer() : base("Row frequencies")
        {
        }

        protected override bool TryGetValue(Key key, out (int, double?) value)
        {
            value = (key.Row, default);
            return true;
        }
    }
}
