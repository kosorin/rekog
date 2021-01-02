using Rekog.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameFingerBigramAnalyzer : BigramAnalyzer<Finger>
    {
        public SameFingerBigramAnalyzer() : base("Same finger bigram")
        {
        }

        protected override List<LayoutAnalysisResult> GroupResultItems(List<(Finger value, LayoutAnalysisResult result)> items)
        {
            return items
                .GroupBy(x => x.value.GetHand())
                .Select(g => new LayoutAnalysisResult(g.Key.ToString(), g.Select(x => new LayoutAnalysisResult(x.value.GetKind().ToString())
                {
                    Percentage = x.result.Percentage,
                    Effort = x.result.Effort,
                }).ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out (Finger, double?) value)
        {
            if (firstKey.Finger == secondKey.Finger && firstKey.Position != secondKey.Position)
            {
                var finger = firstKey.Finger;

                var distance = firstKey.GetDistance(secondKey);
                var effort = distance + 1.5;

                value = (finger, effort);
                return true;
            }
            value = default;
            return false;
        }
    }
}
