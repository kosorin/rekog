using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rekog.Core
{
    public class Alphabet : IEnumerable<char>
    {
        private readonly HashSet<char> _characters;

        public Alphabet() : this("ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
        }

        public Alphabet(IEnumerable<char> characters)
        {
            characters = characters
                .Where(x => !char.IsControl(x) || x == '\t')
                .SelectMany(x => new[] { char.ToLower(x, CultureInfo.InvariantCulture), char.ToUpper(x, CultureInfo.InvariantCulture) })
                .Distinct();
            _characters = new HashSet<char>(characters);
        }

        public bool Contains(char character)
        {
            return _characters.Contains(character);
        }

        public IEnumerator<char> GetEnumerator()
        {
            return _characters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
