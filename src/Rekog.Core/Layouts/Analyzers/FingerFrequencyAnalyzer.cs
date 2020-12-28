namespace Rekog.Core.Layouts.Analyzers
{
    internal class FingerFrequencyAnalyzer : UnigramAnalyzer<Finger>
    {
        public FingerFrequencyAnalyzer() : base("Finger frequencies")
        {
        }

        protected override bool TryAccept(Key key, out Finger value)
        {
            value = key.Finger;
            return true;
        }
    }
}
