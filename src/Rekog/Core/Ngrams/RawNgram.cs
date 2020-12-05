using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Ngrams
{
    public class RawNgram
    {
        public RawNgram(string value)
        {
            Value = value;
        }

        public RawNgram(string value, ulong occurrences) : this(value)
        {
            Occurrences = occurrences;
        }

        public string Value { get; }

        public ulong Occurrences { get; set; }

        public static IEqualityComparer<RawNgram> EqualityComparer { get; } = new RawNgramEqualityComparer();

        private class RawNgramEqualityComparer : IEqualityComparer<RawNgram>
        {
            public bool Equals(RawNgram? x, RawNgram? y)
            {
                return x == y
                    || (x != null && y != null && x.Value == y.Value && x.Occurrences == y.Occurrences);
            }

            public int GetHashCode([DisallowNull] RawNgram obj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
