using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rekog.Core.Extensions;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameFingerBigramAnalyzer : BigramAnalyzer<(Finger, Rune, Rune)>
    {
        public SameFingerBigramAnalyzer()
            : base("Same finger bigram")
        {
        }

        protected override List<LayoutAnalysisResult> GroupResultItems(List<((Finger, Rune, Rune) value, LayoutAnalysisResult result)> items)
        {
            return items
                .GroupBy(x => x.value.Item1.GetHand())
                .Select(h => new LayoutAnalysisResult(h.Key.ToString(), h
                    .GroupBy(x => x.value.Item1.GetKind())
                    .Select(k => new LayoutAnalysisResult(k.Key.ToString(), k
                        .Select(x => new LayoutAnalysisResult(x.value.Item2.ToString() + x.value.Item3.ToString())
                        {
                            Percentage = x.result.Percentage,
                            Effort = x.result.Effort,
                        }).ToList()))
                    .ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out ((Finger, Rune, Rune), double?) value)
        {
            if (firstKey.Finger == secondKey.Finger && firstKey.Position != secondKey.Position)
            {
                var finger = firstKey.Finger;
                var isWeakFinger = finger.GetKind() is FingerKind.Little or FingerKind.Ring;

                var distance = firstKey.GetDistance(secondKey);
                var effort = distance + (isWeakFinger ? 2.5 : 1.5);

                value = ((finger, firstKey.Character, secondKey.Character), effort);
                return true;
            }
            value = default;
            return false;
        }
    }
}
