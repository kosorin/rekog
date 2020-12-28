using Rekog.Core.Corpora;
using Shouldly;
using System.Linq;
using Xunit;

namespace Rekog.Core.UnitTests.Corpora
{
    public class AlphabetTests
    {
        [Fact]
        public void Contains_LowerCase_True()
        {
            var character = 'A';
            var alphabet = new Alphabet(character.ToString());

            alphabet.Contains(char.ToLowerInvariant(character)).ShouldBeTrue();
        }

        [Fact]
        public void Contains_UpperCase_True()
        {
            var character = 'a';
            var alphabet = new Alphabet(character.ToString());

            alphabet.Contains(char.ToUpperInvariant(character)).ShouldBeTrue();
        }

        [Fact]
        public void Enumerable()
        {
            var alphabet = new Alphabet("1abc");

            alphabet.ToArray().ShouldBe("1abcABC", ignoreOrder: true);
        }
    }
}
