namespace Rekog.Core.Layouts.Analyzers
{
    internal class LayerSwitchAnalyzer : BigramAnalyzer<bool>
    {
        public LayerSwitchAnalyzer()
            : base("Layer switch")
        {
        }

        protected override bool TryAnalyze(Key firstKey, Key secondKey, out LayoutNgramAnalysis<bool> result)
        {
            var isSwitch = firstKey.Layer != secondKey.Layer;

            result = new LayoutNgramAnalysis<bool>(isSwitch, isSwitch ? 1 : 0);
            return true;
        }
    }
}
