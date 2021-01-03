using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameFingerMotionAnalyzer : BigramAnalyzer<(Finger finger, Motion motion)>
    {
        public SameFingerMotionAnalyzer() : base("Same finger motion")
        {
        }

        protected override List<LayoutAnalysisResult> GroupResultItems(List<((Finger finger, Motion motion) value, LayoutAnalysisResult result)> items)
        {
            return items
                .GroupBy(x => x.value.motion)
                .Select(g => new LayoutAnalysisResult(g.Key.ToString(), g.Select(x => new LayoutAnalysisResult(x.value.finger.ToString())
                {
                    Percentage = x.result.Percentage,
                    Effort = x.result.Effort,
                }).ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out ((Finger, Motion), double?) value)
        {
            if (firstKey.GetFingerMotion(secondKey) is not Motion.None and Motion fingerMotion)
            {
                value = ((firstKey.Finger, fingerMotion), default);
                return true;
            }
            value = default;
            return false;
        }
    }
}
