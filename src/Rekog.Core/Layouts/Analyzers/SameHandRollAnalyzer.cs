﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameHandRollAnalyzer : BigramAnalyzer<(Hand hand, Roll roll)>
    {
        public SameHandRollAnalyzer() : base("Same hand roll")
        {
        }

        protected override List<LayoutAnalysisResult> GroupResultItems(List<((Hand hand, Roll roll) value, LayoutAnalysisResult result)> items)
        {
            return items
                .GroupBy(x => x.value.roll)
                .Select(g => new LayoutAnalysisResult(g.Key.ToString(), g.Select(x => new LayoutAnalysisResult(x.value.hand.ToString())
                {
                    Percentage = x.result.Percentage,
                    Effort = x.result.Effort,
                }).ToList()))
                .ToList();
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, out ((Hand, Roll), double?) value)
        {
            if (firstKey.GetHandRoll(secondKey) is not Roll.None and Roll handRoll)
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