namespace Rekog.Core.Layouts.Analyzers
{
    internal class LayerSwitchAnalyzer : BigramAnalyzer<bool>
    {
        public LayerSwitchAnalyzer() : base("Layer switch")
        {
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out (bool, double?) value)
        {
            var isSwitch = firstKey.Layer != secondKey.Layer;

            value = (isSwitch, isSwitch ? 0.5 : null);
            return true;
        }
    }
}
