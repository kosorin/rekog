using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rekog.Core.Extensions;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class FingerFrequencyAnalyzer : UnigramAnalyzer<(Finger, Rune)>
    {
        public FingerFrequencyAnalyzer()
            : base("Finger frequencies")
        {
        }

        protected override List<LayoutAnalysisNode> GroupChildren(List<((Finger, Rune) value, LayoutAnalysisNode node)> items)
        {
            return items
                .GroupBy(x => x.value.Item1.GetHand())
                .Select(g => new LayoutAnalysisNode(g.Key.ToString(), g
                    .GroupBy(x => x.value.Item1.GetKind())
                    .Select(k => new LayoutAnalysisNode(k.Key.ToString(), k
                        .Select(x => new LayoutAnalysisNode(x.value.Item2.ToString(), x.node.Percentage, x.node.Effort))
                        .ToList()))
                    .ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key key, out ((Finger, Rune), double?) value)
        {
            value = ((key.Finger, key.Character), key.Effort);
            return true;
        }
    }
}
