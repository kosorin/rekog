namespace Rekog.Core.Layouts.Analyzers
{
    internal class HandAlternationAnalyzer : BigramAnalyzer<bool>
    {
        public HandAlternationAnalyzer()
            : base("Hand alternation")
        {
        }

        protected override bool TryAnalyze(Key firstKey, Key secondKey, out LayoutNgramAnalysis<bool> result)
        {
            var isAlternation = firstKey.Hand != secondKey.Hand;

            result = new LayoutNgramAnalysis<bool>(isAlternation);
            return true;
        }
    }
}
