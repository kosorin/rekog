using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        protected override List<LayoutAnalysisNode> GroupChildren(List<((Finger firstFinger, Finger secondFinger) value, LayoutAnalysisNode node)> items)
        {
            return items
                .GroupBy(x => x.value.firstFinger.GetHand())
                .Select(g => new LayoutAnalysisNode(g.Key.ToString(), g
                    .Select(x => new LayoutAnalysisNode((x.value.firstFinger.GetKind(), x.value.secondFinger.GetKind()).ToString(), x.node.Percentage, x.node.Effort))
                    .ToList()))
                .ToList();
        }

        protected override bool TryAnalyze(Key firstKey, Key secondKey, [MaybeNullWhen(false)] out LayoutNgramAnalysis<(Finger firstFinger, Finger secondFinger)> result)
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

                result = new LayoutNgramAnalysis<(Finger firstFinger, Finger secondFinger)>(fingers, effort);
                return true;
            }
            result = null;
            return false;
        }
    }
}
