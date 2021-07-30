namespace Rekog.Core.Layouts.Analyzers
{
    internal class HomingAnalyzer : UnigramAnalyzer<bool>
    {
        public HomingAnalyzer()
            : base("Homing")
        {
        }

        protected override bool TryAnalyze(Key key, out LayoutNgramAnalysis<bool> result)
        {
            result = new LayoutNgramAnalysis<bool>(key.IsHoming);
            return true;
        }
    }
}
