using System;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Ngrams
{
    public class NgramBuffer : INgramBuffer
    {
        private int _position;
        private int _lastInvalidPosition;
        private readonly char[] _characters;
        private readonly char[] _buffer;

        public NgramBuffer(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Size = size;

            _position = 0;
            _lastInvalidPosition = 0;
            _characters = new char[size];
            _buffer = new char[size];
        }

        public int Size { get; }

        public void Skip()
        {
            _position = GetNextPosition(_position);
            _lastInvalidPosition = _position;
        }

        public bool Next(char character, [MaybeNullWhen(false)] out string ngramValue)
        {
            _position = GetNextPosition(_position);
            _characters[_position] = character;

            if (_position == _lastInvalidPosition)
            {
                _lastInvalidPosition = GetNextPosition(_position);

                var nextPosition = _lastInvalidPosition;
                for (var i = 0; i < Size; i++)
                {
                    _buffer[i] = _characters[(nextPosition + i) % Size];
                }
                ngramValue = new string(_buffer);
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
