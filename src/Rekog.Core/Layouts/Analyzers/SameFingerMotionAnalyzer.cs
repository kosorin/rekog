using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameFingerMotionAnalyzer : BigramAnalyzer<(Finger finger, Motion motion)>
    {
        public SameFingerMotionAnalyzer()
            : base("Same finger motion")
        {
        }

        protected override List<LayoutAnalysisNode> GroupChildren(List<((Finger finger, Motion motion) value, LayoutAnalysisNode node)> items)
        {
            return items
                .GroupBy(x => x.value.motion)
                .Select(g => new LayoutAnalysisNode(g.Key.ToString(), g
                    .Select(x => new LayoutAnalysisNode(x.value.finger.ToString(), x.node.Percentage, x.node.Effort))
                    .ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out ((Finger, Motion), double?) value)
        {
            if (firstKey.GetFingerMotion(secondKey) is not Motion.None and var fingerMotion)
            {
                value = ((firstKey.Finger, fingerMotion), default);
                return true;
            }
            value = default;
            return false;
        }
    }
}
