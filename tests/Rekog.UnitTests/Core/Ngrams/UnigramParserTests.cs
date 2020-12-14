using Rekog.Core.Ngrams;
using Shouldly;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class UnigramParserTests
    {
        [Fact]
        public void Next()
        {
            var parser = new UnigramParser();

            var result1 = parser.Next('x', out var ngramValue1);
            var result2 = parser.Next('y', out var ngramValue2);

            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            ngramValue1.ShouldBe("x");
            ngramValue2.ShouldBe("y");
        }
    }
}
