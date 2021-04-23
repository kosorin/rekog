namespace Rekog.Core.Layouts.Analyzers
{
    internal class HandAlternationAnalyzer : BigramAnalyzer<bool>
    {
        public HandAlternationAnalyzer()
            : base("Hand alternation")
        {
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out (bool, double?) value)
        {
            var isAlternation = firstKey.Hand != secondKey.Hand;

            value = (isAlternation, default);
            return true;
        }
    }
}
