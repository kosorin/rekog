using System;
using System.Linq;

namespace Rekog.Core.Ngram
{
    public class NgramAnalyzerBuffer
    {
        private int _position;
        private readonly char[] _characters;
        private readonly bool[] _states;

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
            _characters = new char[size];
            _states = new bool[size];
        }

        public void Skip()
        {
            Next('\0', out _);
        }

        public bool Next(char character, out string ngramValue)
        {
            _position = GetNextPosition();

            if (!_caseSensitive)
            {
                character = char.ToUpper(character);
            }

            _characters[_position] = character;
            _states[_position] = _alphabet.Contains(character);

            if (IsValidNgram())
            {
                ngramValue = GetNgramValue();
                return true;
            }
            else
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                ngramValue = null;
                return false;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        private bool IsValidNgram()
        {
            return _states.All(x => x);
        }

        private string GetNgramValue()
        {
            var position = GetNextPosition();
            return new string(_characters, position, _size - position) + new string(_characters, 0, position);
        }

        private int GetNextPosition()
        {
            return (_position + 1) % _size;
        }
    }
}
