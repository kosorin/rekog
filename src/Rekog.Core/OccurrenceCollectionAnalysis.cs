using System.Collections.Generic;

namespace Rekog.Core
{
    public class OccurrenceCollectionAnalysis<TValue>
        where TValue : notnull
    {
        internal OccurrenceCollectionAnalysis(IReadOnlyDictionary<TValue, OccurrenceAnalysis<TValue>> occurrences, ulong total, ulong valueTotal, ulong nullTotal)
        {
            Occurrences = occurrences;
            Total = total;
            ValueTotal = valueTotal;
            NullTotal = nullTotal;
        }

        public IReadOnlyDictionary<TValue, OccurrenceAnalysis<TValue>> Occurrences { get; }

        public ulong Total { get; }

        public ulong ValueTotal { get; }

        public ulong NullTotal { get; }
    }
}
