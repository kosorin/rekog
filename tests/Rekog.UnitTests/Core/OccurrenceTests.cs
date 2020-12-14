using Rekog.Core;
using Shouldly;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class OccurrenceTests
    {
        [Fact]
        public void Add()
        {
            var occurrence = new Occurrence<string>("A", 3);
            var expectedCount = 9ul;

            occurrence.Add(6);
            var count = occurrence.Count;

            count.ShouldBe(expectedCount);
        }

        [Fact]
        public void OperatorEquals_Null_True()
        {
            Occurrence<string>? a = null;
            Occurrence<string>? b = null;

            var result = a == b;

            result.ShouldBeTrue();
        }

        [Fact]
        public void OperatorEquals_LeftNull_False()
        {
            Occurrence<string>? a = null;
            var b = new Occurrence<string>("A", 1);

            var result = a == b;

            result.ShouldBeFalse();
        }

        [Fact]
        public void OperatorEquals_RightNull_False()
        {
            var a = new Occurrence<string>("A", 1);
            Occurrence<string>? b = null;

            var result = a == b;

            result.ShouldBeFalse();
        }

        [Fact]
        public void OperatorEquals_SameValueAndCount_True()
        {
            var a = new Occurrence<string>("A", 1);
            var b = new Occurrence<string>("A", 1);

            var result = a == b;

            result.ShouldBeTrue();
        }

        [Fact]
        public void OperatorEquals_DifferentValue_False()
        {
            var a = new Occurrence<string>("A", 1);
            var b = new Occurrence<string>("B", 1);

            var result = a == b;

            result.ShouldBeFalse();
        }

        [Fact]
        public void OperatorEquals_DifferentCount_False()
        {
            var a = new Occurrence<string>("A", 1);
            var b = new Occurrence<string>("A", 2);

            var result = a == b;

            result.ShouldBeFalse();
        }

        [Fact]
        public void OperatorEquals_DifferentValueAndCount_False()
        {
            var a = new Occurrence<string>("A", 1);
            var b = new Occurrence<string>("B", 2);

            var result = a == b;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Equals_Null_False()
        {
            var a = new Occurrence<string>("A", 1);
            Occurrence<string>? b = null;

            var result = a.Equals(b);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Equals_SameValueAndCount_True()
        {
            var a = new Occurrence<string>("A", 1);
            var b = new Occurrence<string>("A", 1);

            var result = a.Equals(b);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Equals_DifferentValue_False()
        {
            var a = new Occurrence<string>("A", 1);
            var b = new Occurrence<string>("B", 1);

            var result = a.Equals(b);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Equals_DifferentCount_False()
        {
            var a = new Occurrence<string>("A", 1);
            var b = new Occurrence<string>("A", 2);

            var result = a == b;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Equals_DifferentValueAndCount_False()
        {
            var a = new Occurrence<string>("A", 1);
            var b = new Occurrence<string>("B", 2);

            var result = a == b;

            result.ShouldBeFalse();
        }

        [Fact]
        public void GetHashCode_DoesNotChangeAfterAdd()
        {
            var occurrence = new Occurrence<string>("A", 300);

            var before = occurrence.GetHashCode();
            occurrence.Add(100);
            var after = occurrence.GetHashCode();

            after.ShouldBe(before);
        }
    }
}
