using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    public class HandFrequencyLayoutAnalyzer : UnigramLayoutAnalyzer<Hand>
    {
        public HandFrequencyLayoutAnalyzer() : base("Hand frequencies")
        {
        }

        protected override bool TryGet(Key key, [MaybeNullWhen(false)] out Hand value)
        {
            value = key.Hand;
            return true;
        }
    }
}
