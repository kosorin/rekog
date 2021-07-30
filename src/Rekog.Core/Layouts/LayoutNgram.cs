using System;

namespace Rekog.Core.Layouts
{
    public class LayoutNgram : IEquatable<LayoutNgram>
    {
        public LayoutNgram(string value, Key[] keys)
        {
            Value = value;
            Keys = keys;
        }

        public string Value { get; }

        public Key[] Keys { get; }

        public bool Equals(LayoutNgram? other)
        {
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return other is not null && EqualsCore(other);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }
            return obj is LayoutNgram other && EqualsCore(other);
        }

        public static bool operator ==(LayoutNgram? left, LayoutNgram? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (left is null)
            {
                return false;
            }
            if (right is null)
            {
                return false;
            }
            return left.EqualsCore(right);
        }

        public static bool operator !=(LayoutNgram? left, LayoutNgram? right)
        {
            return !(left == right);
        }

        private bool EqualsCore(LayoutNgram other)
        {
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
