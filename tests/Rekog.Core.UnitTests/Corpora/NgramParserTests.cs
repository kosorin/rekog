using System;
using System.Text;
using Rekog.Core.Corpora;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests.Corpora
{
    public class NgramParserTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_SizeLessThan1(int size)
        {
            Should.Throw<ArgumentOutOfRangeException>(() => new NgramParser(size));
        }

        [Fact]
        public void Next_Size1()
        {
            var parser = new NgramParser(1);

            var result1 = parser.Next(new Rune('x'), out var ngramValue1);
            var result2 = parser.Next(new Rune('y'), out var ngramValue2);

            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            ngramValue1.ShouldBe("x");
            ngramValue2.ShouldBe("y");
        }

        [Fact]
        public void Next_Size2()
        {
            var parser = new NgramParser(2);

            var result1 = parser.Next(new Rune('x'), out var ngramValue1);
            var result2 = parser.Next(new Rune('y'), out var ngramValue2);
            var result3 = parser.Next(new Rune('z'), out var ngramValue3);
            parser.Skip();
            var result4 = parser.Next(new Rune('a'), out var ngramValue4);
            var result5 = parser.Next(new Rune('b'), out var ngramValue5);

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
            var parser = new NgramParser(4);

            var result1 = parser.Next(new Rune('a'), out var ngramValue1);
            var result2 = parser.Next(new Rune('b'), out var ngramValue2);
            var result3 = parser.Next(new Rune('c'), out var ngramValue3);
            var result4 = parser.Next(new Rune('d'), out var ngramValue4);
            var result5 = parser.Next(new Rune('e'), out var ngramValue5);
            parser.Skip();
            var result6 = parser.Next(new Rune('r'), out var ngramValue6);
            var result7 = parser.Next(new Rune('s'), out var ngramValue7);
            var result8 = parser.Next(new Rune('t'), out var ngramValue8);
            var result9 = parser.Next(new Rune('u'), out var ngramValue9);

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
        public void Next_UnicodeCharacters()
        {
            var parser = new NgramParser(3);

            var result1 = parser.Next(new Rune((char)0xD83C, (char)0xDF04), out var ngramValue1);
            var result2 = parser.Next(new Rune((char)0xD83D, (char)0xDE95), out var ngramValue2);
            var result3 = parser.Next(new Rune((char)0xD83D, (char)0xDE95), out var ngramValue3);
            var result4 = parser.Next(new Rune((char)0x26D4), out var ngramValue4);
            var result5 = parser.Next(new Rune('e'), out var ngramValue5);
            parser.Skip();
            var result6 = parser.Next(new Rune((char)0x26D4), out var ngramValue6);
            var result7 = parser.Next(new Rune((char)0x26D4), out var ngramValue7);
            var result8 = parser.Next(new Rune((char)0xD83C, (char)0xDF04), out var ngramValue8);

            result1.ShouldBeFalse();
            result2.ShouldBeFalse();
            result3.ShouldBeTrue();
            result4.ShouldBeTrue();
            result5.ShouldBeTrue();
            result6.ShouldBeFalse();
            result7.ShouldBeFalse();
            result8.ShouldBeTrue();
            ngramValue1.ShouldBeNull();
            ngramValue2.ShouldBeNull();
            ngramValue3.ShouldBe("\U0001F304\U0001F695\U0001F695");
            ngramValue4.ShouldBe("\U0001F695\U0001F695\u26D4");
            ngramValue5.ShouldBe("\U0001F695\u26D4e");
            ngramValue6.ShouldBeNull();
            ngramValue7.ShouldBeNull();
            ngramValue8.ShouldBe("\u26D4\u26D4\U0001F304");
        }

        [Fact]
        public void Next_MultipleSkip()
        {
            var parser = new NgramParser(2);

            var result1 = parser.Next(new Rune('x'), out var ngramValue1);
            var result2 = parser.Next(new Rune('y'), out var ngramValue2);
            var result3 = parser.Next(new Rune('z'), out var ngramValue3);
            parser.Skip();
            parser.Skip();
            parser.Skip();
            parser.Skip();
            var result4 = parser.Next(new Rune('a'), out var ngramValue4);
            var result5 = parser.Next(new Rune('b'), out var ngramValue5);

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
