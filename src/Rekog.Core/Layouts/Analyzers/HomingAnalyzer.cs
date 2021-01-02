namespace Rekog.Core.Layouts.Analyzers
{
    internal class HomingAnalyzer : UnigramAnalyzer<bool>
    {
        public HomingAnalyzer() : base("Homing")
        {
        }

        protected override bool TryGetValue(Key key, out (bool, double?) value)
        {
            value = (key.IsHoming, default);
            return true;
        }
    }
}
