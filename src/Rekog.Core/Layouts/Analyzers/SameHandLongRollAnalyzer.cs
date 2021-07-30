using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameHandLongRollAnalyzer : TrigramAnalyzer<(Hand hand, Roll roll)>
    {
        public SameHandLongRollAnalyzer()
            : base("Same hand long roll")
        {
        }

        protected override List<LayoutAnalysisNode> GroupChildren(List<((Hand hand, Roll roll) value, LayoutAnalysisNode node)> items)
        {
            return items
                .GroupBy(x => x.value.roll)
                .Select(g => new LayoutAnalysisNode(g.Key.ToString(), g
                    .Select(x => new LayoutAnalysisNode(x.value.hand.ToString(), x.node.Percentage, x.node.Effort))
                    .ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, Key thirdKey, out ((Hand, Roll), double?) value)
        {
            if (firstKey.GetHandRoll(secondKey) is not Roll.None and var handRoll && handRoll == secondKey.GetHandRoll(thirdKey))
            {
                var effort = handRoll switch
                {
                    Roll.Inward => -0.5,
                    Roll.Outward => 0.5,
                    _ => throw new NotSupportedException(),
                };

                value = ((firstKey.Hand, handRoll), effort);
                return true;
            }
            value = default;
            return false;
        }
    }
}
