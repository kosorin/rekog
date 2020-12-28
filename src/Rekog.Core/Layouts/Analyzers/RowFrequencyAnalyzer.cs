namespace Rekog.Core.Layouts.Analyzers
{
    internal class RowFrequencyAnalyzer : UnigramAnalyzer<int>
    {
        public RowFrequencyAnalyzer() : base("Row frequencies")
        {
        }

        protected override bool TryAccept(Key key,out int value)
        {
            value = key.Row;
            return true;
        }
    }
}
