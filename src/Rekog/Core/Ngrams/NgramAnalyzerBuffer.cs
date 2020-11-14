using System;

namespace Rekog.Core.Ngram
{
    public class NgramAnalyzerBuffer
    {
        private int _position;
        private int _lastInvalidPosition;
        private readonly char[] _characters;

        private readonly int _size;
        private readonly bool _caseSensitive;
        private readonly Alphabet _alphabet;

        public NgramAnalyzerBuffer(int size, bool caseSensitive, Alphabet alphabet)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            _size = size;
            _caseSensitive = caseSensitive;
            _alphabet = alphabet;

            _position = 0;
            _lastInvalidPosition = 0;
            _characters = new char[size];
        }

        public void Clear()
        {
            _position = GetNextPosition(_position);
            _lastInvalidPosition = _position;
        }

        public bool Next(char character, out string ngramValue)
        {
            if (!_caseSensitive)
            {
                character = char.ToUpper(character);
            }

            _position = GetNextPosition(_position);

            if (_alphabet.Contains(character))
            {
                _characters[_position] = character;

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
            return (position + 1) % _size;
        }
    }
}
