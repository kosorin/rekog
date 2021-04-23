using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameHandJumpAnalyzer : BigramAnalyzer<(Hand hand, double distance)>
    {
        public SameHandJumpAnalyzer()
            : base("Same hand jump")
        {
        }

        protected override List<LayoutAnalysisResult> GroupResultItems(List<((Hand hand, double distance) value, LayoutAnalysisResult result)> items)
        {
            return items
                .GroupBy(x => x.value.distance)
                .Select(g => new LayoutAnalysisResult(g.Key.ToString(CultureInfo.InvariantCulture), g.Select(x => new LayoutAnalysisResult(x.value.hand.ToString())
                {
                    Percentage = x.result.Percentage,
                    Effort = x.result.Effort,
                }).ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out ((Hand, double), double?) value)
        {
            if (firstKey.Hand == secondKey.Hand)
            {
                value = ((firstKey.Hand, firstKey.GetDistance(secondKey)), default);
                return true;
            }
            value = default;
            return false;
        }
    }
}
