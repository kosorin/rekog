using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests
{
    public class OccurrenceAnalysisTests
    {
        [Fact]
        public void Test()
        {
            var analysis = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 4,
                ["D"] = 8,
                ["E"] = 0,
                ["F"] = 2,
                ["G"] = 2,
                ["H"] = 1,
            }).Analyze();

            AssertOccurrence("A", 5, 0.05_2631);
            AssertOccurrence("B", 5, 0.05_2631);
            AssertOccurrence("C", 2, 0.21_0526);
            AssertOccurrence("D", 1, 0.42_1052);
            AssertOccurrence("E", 8, 0.00_0000);
            AssertOccurrence("F", 3, 0.10_5263);
            AssertOccurrence("G", 3, 0.10_5263);
            AssertOccurrence("H", 5, 0.05_2631);

            void AssertOccurrence(string value, int rank, double percentage)
            {
                var occurrence = analysis.Occurrences[value];
                occurrence.Rank.ShouldBe(rank);
                occurrence.Percentage.ShouldBe(percentage, 0.00_0001);
            }
        }
    }
}
