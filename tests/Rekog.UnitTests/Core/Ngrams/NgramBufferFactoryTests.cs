using Rekog.Core.Ngrams;
using Shouldly;
using System;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramBufferFactoryTests
    {
        [Fact]
        public void Create_Size0_Exception()
        {
            var factory = new NgramBufferFactory();

            Should.Throw<ArgumentOutOfRangeException>(() => factory.Create(0));
        }

        [Fact]
        public void Create_Size1_Unigram()
        {
            var factory = new NgramBufferFactory();

            var buffer = factory.Create(1);

            buffer.ShouldBeOfType<UnigramBuffer>();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Create_SizeGreaterThan1_Unigram(int size)
        {
            var factory = new NgramBufferFactory();

            var buffer = factory.Create(size);

            buffer.ShouldBeOfType<NgramBuffer>();
            ((NgramBuffer)buffer).Size.ShouldBe(size);
        }
    }
}
