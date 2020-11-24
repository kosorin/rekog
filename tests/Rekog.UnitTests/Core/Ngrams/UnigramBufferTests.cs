using Rekog.Core.Ngrams;
using Shouldly;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class UnigramBufferTests
    {
        [Fact]
        public void Next()
        {
            var buffer = new UnigramBuffer();

            var result1 = buffer.Next('x', out var ngramValue1);
            var result2 = buffer.Next('y', out var ngramValue2);

            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            ngramValue1.ShouldBe("x");
            ngramValue2.ShouldBe("y");
        }
    }
}
