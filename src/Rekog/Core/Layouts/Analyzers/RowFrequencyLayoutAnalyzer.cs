using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    public class RowFrequencyLayoutAnalyzer : UnigramLayoutAnalyzer<int>
    {
        public RowFrequencyLayoutAnalyzer() : base("Row frequencies")
        {
        }

        protected override bool TryGet(Key key, [MaybeNullWhen(false)] out int value)
        {
            value = key.Row;
            return true;
        }
    }
}
