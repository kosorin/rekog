using System.Text;
using Rekog.Core.Corpora;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests.Corpora
{
    public class UnigramParserTests
    {
        [Fact]
        public void Next()
        {
            var parser = new UnigramParser();

            var result1 = parser.Next(new Rune('x'), out var ngramValue1);
            var result2 = parser.Next(new Rune('y'), out var ngramValue2);

            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            ngramValue1.ShouldBe("x");
            ngramValue2.ShouldBe("y");
        }

        [Fact]
        public void Next_UnicodeCharacters()
        {
            var parser = new UnigramParser();

            var result1 = parser.Next(new Rune((char)0xD83C, (char)0xDF04), out var ngramValue1);
            var result2 = parser.Next(new Rune((char)0xD83D, (char)0xDE95), out var ngramValue2);

            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            ngramValue1.ShouldBe("\U0001F304");
            ngramValue2.ShouldBe("\U0001F695");
        }
    }
}
