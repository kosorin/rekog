using System.Collections.Generic;
using System.Linq;
using Rekog.Core.Extensions;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class FingerFrequencyAnalyzer : UnigramAnalyzer<Finger>
    {
        public FingerFrequencyAnalyzer()
            : base("Finger frequencies")
        {
        }

        protected override List<LayoutAnalysisResult> GroupResultItems(List<(Finger value, LayoutAnalysisResult result)> items)
        {
            return items
                .GroupBy(x => x.value.GetHand())
                .Select(g => new LayoutAnalysisResult(g.Key.ToString(), g
                    .Select(x => new LayoutAnalysisResult(x.value.GetKind().ToString())
                    {
                        Percentage = x.result.Percentage,
                        Effort = x.result.Effort,
                    }).ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key key, out (Finger, double?) value)
        {
            value = (key.Finger, key.Effort);
            return true;
        }
    }
}
