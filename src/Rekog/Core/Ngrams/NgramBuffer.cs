using System;

namespace Rekog.Core.Ngrams
{
    public class NgramBuffer : INgramBuffer
    {
        private int _position;
        private int _lastInvalidPosition;
        private readonly char[] _characters;

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
        }

        public int Size { get; }

        public void Skip()
        {
            _position = GetNextPosition(_position);
            _lastInvalidPosition = _position;
        }

        public bool Next(char character, out string ngramValue)
        {
            _position = GetNextPosition(_position);
            _characters[_position] = character;

            if (_position == _lastInvalidPosition)
            {
                var nextPosition = GetNextPosition(_position);

                _lastInvalidPosition = nextPosition;

                ngramValue = new string(_characters[nextPosition..]) + new string(_characters[..nextPosition]);
                return true;
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
