using Rekog.Core;
using Shouldly;
using Xunit;

namespace Rekog.UnitTests.Core
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
    }
}
