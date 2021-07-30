using System;

namespace Rekog.Core
{
    public sealed class OccurrenceAnalysis<TValue>
        where TValue : IEquatable<TValue>
    {
        internal OccurrenceAnalysis(TValue value, ulong count, int rank, double percentage)
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
    }
}
