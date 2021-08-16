using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        protected override List<LayoutAnalysisNode> GroupChildren(List<((Finger, Rune, Rune) value, LayoutAnalysisNode node)> items)
        {
            return items
                .GroupBy(x => x.value.Item1.GetHand())
                .Select(h => new LayoutAnalysisNode(h.Key.ToString(), h
                    .GroupBy(x => x.value.Item1.GetKind())
                    .Select(k => new LayoutAnalysisNode(k.Key.ToString(), k
                        .Select(x => new LayoutAnalysisNode(x.value.Item2 + x.value.Item3.ToString(), x.node.Percentage, x.node.Effort))
                        .ToList()))
                    .ToList()))
                .ToList();
        }

        protected override bool TryAnalyze(Key firstKey, Key secondKey, [MaybeNullWhen(false)] out LayoutNgramAnalysis<(Finger, Rune, Rune)> result)
        {
            if (firstKey.Finger == secondKey.Finger && firstKey.Position != secondKey.Position)
            {
                var finger = firstKey.Finger;

                var distance = firstKey.GetDistance(secondKey);
                var effort = distance + 1.5;

                result = new LayoutNgramAnalysis<(Finger, Rune, Rune)>((finger, firstKey.Character, secondKey.Character), effort);
                return true;
            }
            result = null;
            return false;
        }
    }
}
