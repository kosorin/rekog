using System;

namespace Rekog.Core
{
    public class OccurrenceAnal<TValue> : IOccurrenceItem<TValue>, IEquatable<OccurrenceAnal<TValue>>
        where TValue : notnull
    {
        internal OccurrenceAnal(TValue value, ulong count, int rank, double percentage)
        {
            Value = value;
            Count = count;
            Rank = rank;
            Percentage = percentage;
        }

        public TValue Value { get; }

        public ulong Count { get; }

        public int Rank { get; }

        public double Percentage { get; }

        public bool Equals(OccurrenceAnal<TValue>? other)
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
            return obj is OccurrenceAnal<TValue> other && EqualsCore(other);
        }

        public static bool operator ==(OccurrenceAnal<TValue>? left, OccurrenceAnal<TValue>? right)
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

        public static bool operator !=(OccurrenceAnal<TValue>? left, OccurrenceAnal<TValue>? right)
        {
            return !(left == right);
        }

        protected virtual bool EqualsCore(OccurrenceAnal<TValue> other)
        {
            return Count.Equals(other.Count)
                && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
