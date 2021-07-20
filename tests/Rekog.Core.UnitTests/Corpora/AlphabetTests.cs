using System.Linq;
using System.Text;
using Rekog.Core.Corpora;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests.Corpora
{
    public class AlphabetTests
    {
        [Fact]
        public void Contains_LowerCase_True()
        {
            var character = new Rune('A');
            var alphabet = new Alphabet(character.ToString());

            alphabet.Contains(Rune.ToLowerInvariant(character)).ShouldBeTrue();
        }

        [Fact]
        public void Contains_UpperCase_True()
        {
            var character = new Rune('a');
            var alphabet = new Alphabet(character.ToString());

            alphabet.Contains(Rune.ToUpperInvariant(character)).ShouldBeTrue();
        }

        [Fact]
        public void Enumerable()
        {
            var alphabet = new Alphabet("1abc");

            alphabet.ToArray().ShouldBe("1abcABC".EnumerateRunes(), ignoreOrder: true);
        }

        [Fact]
        public void Enumerable_UnicodeCharacters()
        {
            var alphabet = new Alphabet("a🚕🌄🎨𐑉");

            alphabet.ToArray().ShouldBe("aA🚕🌄🎨𐑉𐐡".EnumerateRunes(), ignoreOrder: true);
        }
    }
}
