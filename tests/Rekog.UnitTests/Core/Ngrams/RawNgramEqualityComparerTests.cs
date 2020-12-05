using Rekog.Core.Ngrams;
using Shouldly;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class RawNgramEqualityComparerTests
    {
        [Fact]
        public void Equals_True()
        {
            var comparer = RawNgram.EqualityComparer;
            var a = new RawNgram("A", 1);
            var b = new RawNgram("A", 1);

            var result = comparer.Equals(a, b);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Equals_Null_True()
        {
            var comparer = RawNgram.EqualityComparer;
            RawNgram? a = null;
            RawNgram? b = null;

            var result = comparer.Equals(a, b);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Equals_Value_False()
        {
            var comparer = RawNgram.EqualityComparer;
            var a = new RawNgram("A", 1);
            var b = new RawNgram("B", 1);

            var result = comparer.Equals(a, b);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Equals_Occurrences_False()
        {
            var comparer = RawNgram.EqualityComparer;
            var a = new RawNgram("A", 1);
            var b = new RawNgram("A", 2);

            var result = comparer.Equals(a, b);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Equals_ValueAndOccurrences_False()
        {
            var comparer = RawNgram.EqualityComparer;
            var a = new RawNgram("A", 1);
            var b = new RawNgram("B", 2);

            var result = comparer.Equals(a, b);

            result.ShouldBeFalse();
        }
    }
}
