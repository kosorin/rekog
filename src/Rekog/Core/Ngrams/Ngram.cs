using System;

namespace Rekog.Core.Ngrams
{
    public class Ngram : IEquatable<Ngram>
    {
        private RawNgram? _raw;

        public Ngram(string value, int rank, double percentage, ulong occurrences)
        {
            Value = value;
            Rank = rank;
            Percentage = percentage;
            Occurrences = occurrences;
        }

        public RawNgram Raw => _raw ??= new RawNgram
        {
            Value = Value,
            Occurrences = Occurrences
        };

        public string Value { get; }

        public int Rank { get; }

        public double Percentage { get; }

        public ulong Occurrences { get; }

        public static implicit operator string(Ngram? ngram)
        {
            return ngram?.Value ?? throw new NullReferenceException();
        }

        public override string ToString()
        {
            return Value;
        }

        public bool Equals(Ngram? other)
        {
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return EqualsCore(other);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            return obj is Ngram other && EqualsCore(other);
        }

        public static bool operator ==(Ngram? left, Ngram? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(left, null))
            {
                return false;
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }
            return left.EqualsCore(right);
        }

        public static bool operator !=(Ngram? left, Ngram? right)
        {
            return !(left == right);
        }

        public static bool operator ==(Ngram? left, string? right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }
            return left.EqualsValue(right);
        }

        public static bool operator !=(Ngram? left, string? right)
        {
            return !(left == right);
        }

        public static bool operator ==(string? left, Ngram? right)
        {
            if (ReferenceEquals(right, null))
            {
                return ReferenceEquals(left, null);
            }
            if (ReferenceEquals(left, null))
            {
                return false;
            }
            return right.EqualsValue(left);
        }

        public static bool operator !=(string? left, Ngram? right)
        {
            return !(left == right);
        }

        protected virtual bool EqualsCore(Ngram other)
        {
            return EqualsValue(other.Value);
        }

        protected virtual bool EqualsValue(string otherValue)
        {
            return Value.Equals(otherValue);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
