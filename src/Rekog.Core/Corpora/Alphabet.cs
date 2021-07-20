using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rekog.Core.Corpora
{
    public class Alphabet : IEnumerable<Rune>
    {
        private readonly HashSet<Rune> _characters;

        public Alphabet(string characters)
        {
            _characters = characters
                .EnumerateRunes()
                .SelectMany(x => new[] { Rune.ToLowerInvariant(x), Rune.ToUpperInvariant(x), })
                .Distinct()
                .ToHashSet();
        }

        public bool Contains(Rune character)
        {
            return _characters.Contains(character);
        }

        public IEnumerator<Rune> GetEnumerator()
        {
            return _characters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
