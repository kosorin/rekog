namespace Rekog.Core
{
    public class OccurrenceAnalysis<TValue>
        where TValue : notnull
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
