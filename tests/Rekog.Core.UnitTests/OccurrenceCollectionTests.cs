using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests
{
    public class OccurrenceCollectionTests
    {
        [Fact]
        public void AddNull()
        {
            var occurrences = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 2,
            });
            var expectedTotal = 5ul;

            occurrences.AddNull();
            var total = occurrences.Total;

            total.ShouldBe(expectedTotal);
        }

        [Fact]
        public void AddNull_Value()
        {
            var occurrences = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 2,
            });
            var expectedTotal = 14ul;

            occurrences.AddNull(10);
            var total = occurrences.Total;

            total.ShouldBe(expectedTotal);
        }

        [Fact]
        public void Add_Value()
        {
            var occurrences = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 2,
            });
            var expectedOccurrenceCount = 3ul;
            var expectedTotal = 5ul;

            occurrences.Add("C");
            var occurrenceCount = occurrences["C"].Count;
            var total = occurrences.Total;

            occurrenceCount.ShouldBe(expectedOccurrenceCount);
            total.ShouldBe(expectedTotal);
        }

        [Fact]
        public void Add_ValueCount()
        {
            var occurrences = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 2,
            });
            var expectedOccurrenceCount = 5ul;
            var expectedTotal = 7ul;

            occurrences.Add("C", 3);
            var occurrenceCount = occurrences["C"].Count;
            var total = occurrences.Total;

            occurrenceCount.ShouldBe(expectedOccurrenceCount);
            total.ShouldBe(expectedTotal);
        }

        [Fact]
        public void Add_NewValue()
        {
            var occurrences = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 2,
            });
            var expectedOccurrenceCount = 3ul;
            var expectedTotal = 7ul;

            occurrences.Add("D", 3);
            var occurrenceCount = occurrences["D"].Count;
            var total = occurrences.Total;

            occurrenceCount.ShouldBe(expectedOccurrenceCount);
            total.ShouldBe(expectedTotal);
        }

        [Fact]
        public void Add_OccurrenceCollection()
        {
            var occurrences1 = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 2,
            });
            var occurrences2 = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["C"] = 1,
                ["D"] = 1,
            });
            var expectedOccurrences = new[]
            {
                new Occurrence<string>("A", 1),
                new Occurrence<string>("B", 1),
                new Occurrence<string>("C", 3),
                new Occurrence<string>("D", 1),
            };

            occurrences1.Add(occurrences2);

            occurrences1.ShouldBe(expectedOccurrences, ignoreOrder: true);
        }

        [Fact]
        public void Add_OccurrenceCollection_Self_ThrowException()
        {
            var occurrences = new OccurrenceCollection<string>(new Dictionary<string, ulong>
            {
                ["A"] = 1,
                ["B"] = 1,
                ["C"] = 2,
            });

            Should.Throw<ArgumentException>(() => occurrences.Add(occurrences));
        }
    }
}
