using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    public class FingerFrequencyLayoutAnalyzer : UnigramLayoutAnalyzer<Finger>
    {
        public FingerFrequencyLayoutAnalyzer() : base("Finger frequencies")
        {
        }

        protected override bool TryGet(Key key, [MaybeNullWhen(false)] out Finger value)
        {
            value = key.Finger;
            return true;
        }
    }
}
