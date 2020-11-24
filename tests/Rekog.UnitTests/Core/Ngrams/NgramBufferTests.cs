using Rekog.Core.Ngrams;
using Shouldly;
using System;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramBufferTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_SizeLessThan1(int size)
        {
            Should.Throw<ArgumentOutOfRangeException>(() => new NgramBuffer(size));
        }

        [Fact]
        public void Next_Size1()
        {
            var buffer = new NgramBuffer(1);

            var result1 = buffer.Next('x', out var ngramValue1);
            var result2 = buffer.Next('y', out var ngramValue2);

            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            ngramValue1.ShouldBe("x");
            ngramValue2.ShouldBe("y");
        }

        [Fact]
        public void Next_Size2()
        {
            var buffer = new NgramBuffer(2);

            var result1 = buffer.Next('x', out var ngramValue1);
            var result2 = buffer.Next('y', out var ngramValue2);
            var result3 = buffer.Next('z', out var ngramValue3);
            buffer.Skip();
            var result4 = buffer.Next('a', out var ngramValue4);
            var result5 = buffer.Next('b', out var ngramValue5);

            result1.ShouldBeFalse();
            result2.ShouldBeTrue();
            result3.ShouldBeTrue();
            result4.ShouldBeFalse();
            result5.ShouldBeTrue();
            ngramValue1.ShouldBeNull();
            ngramValue2.ShouldBe("xy");
            ngramValue3.ShouldBe("yz");
            ngramValue4.ShouldBeNull();
            ngramValue5.ShouldBe("ab");
        }

        [Fact]
        public void Next_Size4()
        {
            var buffer = new NgramBuffer(4);

            var result1 = buffer.Next('a', out var ngramValue1);
            var result2 = buffer.Next('b', out var ngramValue2);
            var result3 = buffer.Next('c', out var ngramValue3);
            var result4 = buffer.Next('d', out var ngramValue4);
            var result5 = buffer.Next('e', out var ngramValue5);
            buffer.Skip();
            var result6 = buffer.Next('r', out var ngramValue6);
            var result7 = buffer.Next('s', out var ngramValue7);
            var result8 = buffer.Next('t', out var ngramValue8);
            var result9 = buffer.Next('u', out var ngramValue9);

            result1.ShouldBeFalse();
            result2.ShouldBeFalse();
            result3.ShouldBeFalse();
            result4.ShouldBeTrue();
            result5.ShouldBeTrue();
            result6.ShouldBeFalse();
            result7.ShouldBeFalse();
            result8.ShouldBeFalse();
            result9.ShouldBeTrue();
            ngramValue1.ShouldBeNull();
            ngramValue2.ShouldBeNull();
            ngramValue3.ShouldBeNull();
            ngramValue4.ShouldBe("abcd");
            ngramValue5.ShouldBe("bcde");
            ngramValue6.ShouldBeNull();
            ngramValue7.ShouldBeNull();
            ngramValue8.ShouldBeNull();
            ngramValue9.ShouldBe("rstu");
        }

        [Fact]
        public void Next_MultipleSkip()
        {
            var buffer = new NgramBuffer(2);

            var result1 = buffer.Next('x', out var ngramValue1);
            var result2 = buffer.Next('y', out var ngramValue2);
            var result3 = buffer.Next('z', out var ngramValue3);
            buffer.Skip();
            buffer.Skip();
            buffer.Skip();
            buffer.Skip();
            var result4 = buffer.Next('a', out var ngramValue4);
            var result5 = buffer.Next('b', out var ngramValue5);

            result1.ShouldBeFalse();
            result2.ShouldBeTrue();
            result3.ShouldBeTrue();
            result4.ShouldBeFalse();
            result5.ShouldBeTrue();
            ngramValue1.ShouldBeNull();
            ngramValue2.ShouldBe("xy");
            ngramValue3.ShouldBe("yz");
            ngramValue4.ShouldBeNull();
            ngramValue5.ShouldBe("ab");
        }
    }
}
