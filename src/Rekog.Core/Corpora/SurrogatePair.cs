using System;

namespace Rekog.Core.Corpora
{
    public readonly struct SurrogatePair : IEquatable<SurrogatePair>
    {
        public SurrogatePair(char high, char low)
        {
            High = high;
            Low = low;
            CodePoint = char.ConvertToUtf32(High, Low);
        }

        public char High { get; }

        public char Low { get; }

        public int CodePoint { get; }

        public string ConvertToString()
        {
            return char.ConvertFromUtf32(CodePoint);
        }

        public bool Equals(SurrogatePair other)
        {
            return CodePoint == other.CodePoint;
        }

        public override bool Equals(object? obj)
        {
            return obj is SurrogatePair other && Equals(other);
        }

        public override int GetHashCode()
        {
            return CodePoint;
        }

        public static bool operator ==(SurrogatePair left, SurrogatePair right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SurrogatePair left, SurrogatePair right)
        {
            return !left.Equals(right);
        }
    }
}
