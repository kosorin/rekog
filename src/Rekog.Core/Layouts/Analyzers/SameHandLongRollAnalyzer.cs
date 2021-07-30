﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameHandLongRollAnalyzer : TrigramAnalyzer<(Hand hand, Direction roll)>
    {
        public SameHandLongRollAnalyzer()
            : base("Same hand long roll")
        {
        }

        protected override List<LayoutAnalysisNode> GroupChildren(List<((Hand hand, Direction roll) value, LayoutAnalysisNode node)> items)
        {
            return items
                .GroupBy(x => x.value.roll)
                .Select(g => new LayoutAnalysisNode(g.Key.ToString(), g
                    .Select(x => new LayoutAnalysisNode(x.value.hand.ToString(), x.node.Percentage, x.node.Effort))
                    .ToList()))
                .ToList();
        }

        protected override bool TryAnalyze(Key firstKey, Key secondKey, Key thirdKey, [MaybeNullWhen(false)] out LayoutNgramAnalysis<(Hand hand, Direction roll)> result)
        {
            if (firstKey.GetHandRoll(secondKey) is not Direction.None and var handRoll && handRoll == secondKey.GetHandRoll(thirdKey))
            {
                var effort = handRoll switch
                {
                    Direction.Inward => -0.5,
                    Direction.Outward => 0.5,
                    _ => throw new NotSupportedException(),
                };

                result = new LayoutNgramAnalysis<(Hand hand, Direction roll)>((firstKey.Hand, handRoll), effort);
                return true;
            }
            result = null;
            return false;
        }
    }
}
