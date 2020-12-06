using Rekog.Core.Ngrams;
using Shouldly;
using System;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramCollectorTests
    {
        [Fact]
        public void Append()
        {
            var collector1 = new NgramCollector(new UnigramBuffer());
            collector1.Next('a');
            collector1.Next('b');
            collector1.Next('c');
            collector1.Next('c');
            var collector2 = new NgramCollector(new UnigramBuffer());
            collector2.Next('c');
            collector2.Next('d');

            collector1.Append(collector2);
            collector1.Append(collector2);
            var expectedRawNgrams = new[]
            {
                new RawNgram("a", 1),
                new RawNgram("b", 1),
                new RawNgram("c", 4),
                new RawNgram("d", 2),
            };

            var rawNgrams = collector1.GetNgrams().ToRawNgrams();

            rawNgrams.ShouldBe(expectedRawNgrams, RawNgram.EqualityComparer, ignoreOrder: true);
        }

        [Fact]
        public void Append_Self_ThrowException()
        {
            var collector = new NgramCollector(new UnigramBuffer());

            Should.Throw<ArgumentException>(() => collector.Append(collector));
        }

        [Fact]
        public void GetNgrams()
        {
            var buffer = new NgramBuffer(2);
            var collector = new NgramCollector(buffer);
            var corpus = "abcd ef efx";
            var expectedRawNgrams = new[]
            {
                new RawNgram("ab", 1),
                new RawNgram("bc", 1),
                new RawNgram("cd", 1),
                new RawNgram("ef", 2),
                new RawNgram("fx", 1),
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
            var rawNgrams = collector.GetNgrams().ToRawNgrams();

            rawNgrams.ShouldBe(expectedRawNgrams, RawNgram.EqualityComparer, ignoreOrder: true);
        }
    }
}
