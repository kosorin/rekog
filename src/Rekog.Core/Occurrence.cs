using System;

namespace Rekog.Core
{
    public class Occurrence<TValue> : IEquatable<Occurrence<TValue>>, IComparable<Occurrence<TValue>>
        where TValue : notnull
    {
        public Occurrence(TValue value)
        {
            Value = value;
        }

        public Occurrence(TValue value, ulong count)
            : this(value)
        {
            Count = count;
        }

        public TValue Value { get; }

        public ulong Count { get; private set; }

        public void Add(ulong count)
        {
            Count += count;
        }

        public bool Equals(Occurrence<TValue>? other)
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
            return obj is Occurrence<TValue> other && EqualsCore(other);
        }

        public static bool operator ==(Occurrence<TValue>? left, Occurrence<TValue>? right)
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

        public static bool operator !=(Occurrence<TValue>? left, Occurrence<TValue>? right)
        {
            return !(left == right);
        }

        protected virtual bool EqualsCore(Occurrence<TValue> other)
        {
            return Count.Equals(other.Count)
                && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(Occurrence<TValue>? other)
        {
            if (ReferenceEquals(other, this))
            {
                return 0;
            }
            if (other is null)
            {
                return 1;
            }
            return Count.CompareTo(other.Count);
        }
    }
}
