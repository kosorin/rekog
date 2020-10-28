using System;
using System.Linq;

namespace Rekog.Commands.Ngram
{
    public class NgramAnalyzeBuffer
    {
        private int _position;

        private readonly char[] _characters;
        private readonly bool[] _states;

        public NgramAnalyzeBuffer(int size, bool caseSensitive, string? alphabet)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            Size = size;
            CaseSensitive = caseSensitive;
            Alphabet = caseSensitive ? alphabet : alphabet?.ToUpper();
            _characters = new char[size];
            _states = new bool[size];
        }

        public int Size { get; }

        public bool CaseSensitive { get; }

        public string? Alphabet { get; }

        public string? Add(char ch)
        {
            MoveNext();

            if (!CaseSensitive)
            {
                ch = char.ToUpper(ch);
            }

            _characters[_position] = ch;
            _states[_position] = Alphabet != null
                ? Alphabet.Contains(ch)
                : char.IsLetter(ch);

            if (IsValid())
            {
                return GetNgram();
            }
            else
            {
                return null;
            }
        }

        private bool IsValid()
        {
            return _states.All(x => x);
        }

        private string GetNgram()
        {
            var position = (_position + 1) % Size;
            return new string(_characters, position, Size - position) + new string(_characters, 0, position);
        }

        private void MoveNext()
        {
            _position = (_position + 1) % Size;
        }
    }
}
