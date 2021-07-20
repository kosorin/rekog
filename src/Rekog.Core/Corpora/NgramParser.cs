using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Rekog.Core.Corpora
{
    public class NgramParser : INgramParser
    {
        private int _position;
        private int _lastInvalidPosition;
        private readonly Rune[] _characters;
        private readonly char[] _buffer;

        public NgramParser(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Size = size;

            _position = 0;
            _lastInvalidPosition = 0;
            _characters = new Rune[size];
            _buffer = new char[size * 2]; // * 2 because Rune can be 1 or 2 long
        }

        public int Size { get; }

        public void Skip()
        {
            _position = GetNextPosition(_position);
            _lastInvalidPosition = _position;
        }

        public bool Next(Rune character, [MaybeNullWhen(false)] out string ngramValue)
        {
            _position = GetNextPosition(_position);
            _characters[_position] = character;

            if (_position == _lastInvalidPosition)
            {
                _lastInvalidPosition = GetNextPosition(_position);

                var nextPosition = _lastInvalidPosition;
                var bufferLength = 0;
                for (var i = 0; i < Size; i++)
                {
                    var bufferCharacter = _characters[(nextPosition + i) % Size];
                    bufferLength += bufferCharacter.EncodeToUtf16(_buffer.AsSpan(bufferLength));
                }
                ngramValue = new string(_buffer, 0, bufferLength);
                return true;
            }

            ngramValue = null;
            return false;
        }

        private int GetNextPosition(int position)
        {
            return (position + 1) % Size;
        }
    }
}
