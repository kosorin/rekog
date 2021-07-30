using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        protected override bool TryAnalyze(Key firstKey, Key secondKey, [MaybeNullWhen(false)] out LayoutNgramAnalysis<(Finger finger, Motion motion)> result)
        {
            if (firstKey.GetFingerMotion(secondKey) is not Motion.None and var fingerMotion)
            {
                result = new LayoutNgramAnalysis<(Finger finger, Motion motion)>((firstKey.Finger, fingerMotion));
                return true;
            }
            result = null;
            return false;
        }
    }
}
