namespace Rekog.Core.Layouts.Analyzers
{
    internal class HandFrequencyAnalyzer : UnigramAnalyzer<Hand>
    {
        public HandFrequencyAnalyzer() : base("Hand frequencies")
        {
        }

        protected override bool TryAccept(Key key, out Hand value)
        {
            value = key.Hand;
            return true;
        }
    }
}
