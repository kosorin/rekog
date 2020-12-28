using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class HandAlternationAnalyzer : BigramAnalyzer<bool>
    {
        public HandAlternationAnalyzer() : base("Hand alternation")
        {
        }

        protected override bool TryAccept(Key firstKey, Key secondKey, [MaybeNullWhen(false)] out bool value)
        {
            value = firstKey.Hand != secondKey.Hand;
            return true;
        }
    }
}
