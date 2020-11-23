using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rekog.Core
{
    public class Alphabet : IEnumerable<char>
    {
        private readonly HashSet<char> _characters;

        // TODO: Remove parameterless constructor
        public Alphabet() : this("ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
        }

        public Alphabet(IEnumerable<char> characters)
        {
            characters = characters
                .SelectMany(x => new[] { char.ToLowerInvariant(x), char.ToUpperInvariant(x) })
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
