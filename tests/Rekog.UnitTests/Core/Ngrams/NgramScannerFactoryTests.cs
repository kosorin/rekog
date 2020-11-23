using Rekog.Core;
using Rekog.Core.Ngrams;
using Shouldly;
using System;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramScannerFactoryTests
    {
        [Fact]
        public void Create_Size0_Exception()
        {
            var factory = new NgramScannerFactory();
            var alphabet = new Alphabet("ABCDEF");

            Should.Throw<ArgumentOutOfRangeException>(() => factory.Create(0, false, alphabet));
        }

        [Fact]
        public void Create_Size1_Unigram()
        {
            var factory = new NgramScannerFactory();
            var alphabet = new Alphabet("ABCDEF");

            var scanner = factory.Create(1, false, alphabet);

            scanner.ShouldBeOfType<UnigramScanner>();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Create_SizeGreaterThan1_Unigram(int size)
        {
            var factory = new NgramScannerFactory();
            var alphabet = new Alphabet("ABCDEF");

            var scanner = factory.Create(size, false, alphabet);

            scanner.ShouldBeOfType<NgramScanner>();
            scanner.Size.ShouldBe(size);
        }
    }
}
