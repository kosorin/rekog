using Rekog.Core;
using System.Globalization;
using System.Linq;
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

            var containsLowerCase = alphabet.Contains(char.ToLower(character, CultureInfo.InvariantCulture));

            Assert.True(containsLowerCase);
        }

        [Fact]
        public void Contains_UpperCase_True()
        {
            var character = 'a';
            var alphabet = new Alphabet(character.ToString());

            var containsUpperCase = alphabet.Contains(char.ToUpper(character, CultureInfo.InvariantCulture));

            Assert.True(containsUpperCase);
        }

        [Fact]
        public void Contains_Tab_True()
        {
            var character = '\t';
            var alphabet = new Alphabet(character.ToString());

            var containsTab = alphabet.Contains(character);

            Assert.True(containsTab);
        }

        [Fact]
        public void Contains_ControlCharacters_False()
        {
            var characters = "\n\r\0\f\a\b";
            var alphabet = new Alphabet(characters);

            var containsControlCharacters = characters.Any(x => alphabet.Contains(x));

            Assert.False(containsControlCharacters);
        }
    }
}
