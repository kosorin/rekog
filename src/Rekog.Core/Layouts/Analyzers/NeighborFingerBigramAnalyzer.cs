using System.Collections.Generic;
using System.Linq;
using Rekog.Core.Extensions;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class NeighborFingerBigramAnalyzer : BigramAnalyzer<(Finger firstFinger, Finger secondFinger)>
    {
        public NeighborFingerBigramAnalyzer()
            : base("Neighbor finger bigram")
        {
        }

        protected override List<LayoutAnalysisResult> GroupResultItems(List<((Finger firstFinger, Finger secondFinger) value, LayoutAnalysisResult result)> items)
        {
            return items
                .GroupBy(x => x.value.firstFinger.GetHand())
                .Select(g => new LayoutAnalysisResult(g.Key.ToString(), g.Select(x => new LayoutAnalysisResult((x.value.firstFinger.GetKind(), x.value.secondFinger.GetKind()).ToString())
                {
                    Percentage = x.result.Percentage,
                    Effort = x.result.Effort,
                }).ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out ((Finger, Finger), double?) value)
        {
            if (firstKey.Finger.IsNeighbor(secondKey.Finger))
            {
                var fingers = (firstKey.Finger, secondKey.Finger);

                var distance = firstKey.GetDistance(secondKey);
                var effort = distance * (firstKey.Finger.GetKind(), secondKey.Finger.GetKind()) switch
                {
                    (FingerKind.Little, FingerKind.Ring) or (FingerKind.Ring, FingerKind.Little) => 0.5,
                    (FingerKind.Ring, FingerKind.Middle) or (FingerKind.Middle, FingerKind.Ring) => 0.1,
                    _ => 0,
                };

                value = (fingers, effort);
                return true;
            }
            value = default;
            return false;
        }
    }
}
