﻿using Rekog.Core;
using Rekog.Core.Ngrams;
using Shouldly;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramCollectorTests
    {
        [Fact]
        public void Occurrences()
        {
            var parser = new NgramParser(2);
            var collector = new NgramCollector(parser);
            var corpus = "abcd ef efx";
            var expectedNgramOccurrences = new[]
            {
                new Occurrence<string>("ab", 1),
                new Occurrence<string>("bc", 1),
                new Occurrence<string>("cd", 1),
                new Occurrence<string>("ef", 2),
                new Occurrence<string>("fx", 1),
            };

            foreach (var character in corpus)
            {
                if (char.IsWhiteSpace(character))
                {
                    collector.Skip();
                }
                else
                {
                    collector.Next(character);
                }
            }
            var ngramOccurrences = collector.Occurrences;

            ngramOccurrences.ShouldBe(expectedNgramOccurrences, ignoreOrder: true);
        }
    }
}
