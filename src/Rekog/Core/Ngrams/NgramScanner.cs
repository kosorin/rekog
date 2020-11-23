using System;

namespace Rekog.Core.Ngrams
{
    public class NgramScanner : INgramScanner
    {
        private int _position;
        private int _lastInvalidPosition;
        private readonly char[] _characters;

        public NgramScanner(int size, bool caseSensitive, Alphabet alphabet)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Size = size;
            CaseSensitive = caseSensitive;
            Alphabet = alphabet;

            _position = 0;
            _lastInvalidPosition = 0;
            _characters = new char[size];
        }

        public int Size { get; }

        public bool CaseSensitive { get; }

        public Alphabet Alphabet { get; }

        public void Clear()
        {
            _position = GetNextPosition(_position);
            _lastInvalidPosition = _position;
        }

        public bool Next(char character, out string ngramValue)
        {
            _position = GetNextPosition(_position);

            if (Alphabet.Contains(character))
            {
                _characters[_position] = CaseSensitive ? character : char.ToUpperInvariant(character);

                if (_position == _lastInvalidPosition)
                {
                    var nextPosition = GetNextPosition(_position);

                    _lastInvalidPosition = nextPosition;

                    ngramValue = new string(_characters[nextPosition..]) + new string(_characters[..nextPosition]);
                    return true;
                }
            }
            else
            {
                _lastInvalidPosition = _position;
            }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ngramValue = null;
            return false;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        private int GetNextPosition(int position)
        {
            return (position + 1) % Size;
        }
    }
}
