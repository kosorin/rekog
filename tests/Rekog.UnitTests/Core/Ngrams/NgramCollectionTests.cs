using Rekog.Core.Ngrams;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramCollectionTests
    {
        [Fact]
        public void GeneratedStatistics()
        {
            var rawNgrams = new[]
            {
                new RawNgram("A", 1),
                new RawNgram("B", 1),
                new RawNgram("C", 4),
                new RawNgram("D", 8),
                new RawNgram("E", 0),
                new RawNgram("F", 2),
                new RawNgram("G", 2),
                new RawNgram("H", 1),
            };
            var ngrams = new NgramCollection(rawNgrams);

            AssertNgram("A", 5, 0.05_2631);
            AssertNgram("B", 5, 0.05_2631);
            AssertNgram("C", 2, 0.21_0526);
            AssertNgram("D", 1, 0.42_1052);
            AssertNgram("E", 8, 0.00_0000);
            AssertNgram("F", 3, 0.10_5263);
            AssertNgram("G", 3, 0.10_5263);
            AssertNgram("H", 5, 0.05_2631);

            void AssertNgram(string ngramValue, int rank, double percentage)
            {
                var ngram = ngrams[ngramValue];
                ngram.Rank.ShouldBe(rank);
                ngram.Percentage.ShouldBe(percentage, 0.00_0001);
            }
        }

        [Fact]
        public void Ctor_SameNgrams_ThrowException()
        {
            var rawNgrams = new[]
            {
                new RawNgram("A"),
                new RawNgram("A"),
            };

            Should.Throw<ArgumentException>(() => new NgramCollection(rawNgrams));
        }

        [Fact]
        public void Ctor_DifferentSize_ThrowException()
        {
            var rawNgrams = new[]
            {
                new RawNgram("A"),
                new RawNgram("AA"),
            };

            Should.Throw<FormatException>(() => new NgramCollection(rawNgrams));
        }

        [Fact]
        public void Size()
        {
            var ngrams = TestNgrams.Instance;
            var expectedSize = 1;

            var size = ngrams.Size;

            size.ShouldBe(expectedSize);
        }

        [Fact]
        public void TotalOccurrences()
        {
            var ngrams = TestNgrams.Instance;
            var expectedTotalOccurrences = 4ul;

            var totalOccurrences = ngrams.TotalOccurrences;

            totalOccurrences.ShouldBe(expectedTotalOccurrences);
        }

        [Fact]
        public void Count()
        {
            var ngrams = TestNgrams.Instance;
            var expectedCount = 3;

            var count = ngrams.Count;

            count.ShouldBe(expectedCount);
        }

        [Fact]
        public void Indexer()
        {
            var ngrams = TestNgrams.Instance;
            var expectedNgram = TestNgrams.A;

            var ngram = ngrams["A"];

            ngram.ShouldBe(expectedNgram);
        }

        [Fact]
        public void Indexer_ThrowException()
        {
            var ngrams = TestNgrams.Instance;

            Should.Throw<KeyNotFoundException>(() => ngrams["X"]);
        }

        [Fact]
        public void ContainsKey_True()
        {
            var ngrams = TestNgrams.Instance;

            var result = ngrams.ContainsKey("A");

            result.ShouldBeTrue();
        }

        [Fact]
        public void ContainsKey_False()
        {
            var ngrams = TestNgrams.Instance;

            var result = ngrams.ContainsKey("X");

            result.ShouldBeFalse();
        }

        [Fact]
        public void TryGetValue_True()
        {
            var ngrams = TestNgrams.Instance;

            var result = ngrams.TryGetValue("A", out var ngram);

            result.ShouldBeTrue();
            ngram.ShouldBe(TestNgrams.A);
        }

        [Fact]
        public void TryGetValue_False()
        {
            var ngrams = TestNgrams.Instance;

            var result = ngrams.TryGetValue("X", out var ngram);

            result.ShouldBeFalse();
            ngram.ShouldBe(null);
        }

        [Fact]
        public void Keys()
        {
            var ngrams = TestNgrams.Instance;
            var expectedKeys = new[] { "A", "B", "C" };

            var keys = ngrams.Keys;

            keys.ShouldBe(expectedKeys, ignoreOrder: true);
        }

        [Fact]
        public void Values()
        {
            var ngrams = TestNgrams.Instance;
            var expectedValues = new[] { TestNgrams.A, TestNgrams.B, TestNgrams.C };

            var values = ngrams.Values;

            values.ShouldBe(expectedValues, ignoreOrder: true);
        }

        [Fact]
        public void EnumerableToList()
        {
            var ngrams = TestNgrams.Instance;
            var expectedItems = new[]
            {
                new KeyValuePair<string, NgramCollection.Ngram>("A", TestNgrams.A),
                new KeyValuePair<string, NgramCollection.Ngram>("B", TestNgrams.B),
                new KeyValuePair<string, NgramCollection.Ngram>("C", TestNgrams.C),
            };

            var items = ngrams.ToList();

            items.ShouldBe(expectedItems, ignoreOrder: true);
        }

        private static class TestNgrams
        {
            static TestNgrams()
            {
                Instance = new NgramCollection(new[]
                {
                    new RawNgram("A", 1),
                    new RawNgram("B", 1),
                    new RawNgram("C", 2),
                });
                A = new NgramCollection.Ngram("A", 2, 0.25, 1);
                B = new NgramCollection.Ngram("B", 2, 0.25, 1);
                C = new NgramCollection.Ngram("C", 1, 0.5, 2);
            }

            public static NgramCollection Instance { get; }

            public static NgramCollection.Ngram A { get; }

            public static NgramCollection.Ngram B { get; }

            public static NgramCollection.Ngram C { get; }
        }
    }
}
