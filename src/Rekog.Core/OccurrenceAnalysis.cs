using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rekog.Core
{
    public class OccurrenceAnalysis<TValue> : IOccurrenceMap<TValue, OccurrenceAnalysis<TValue>.Occurrence>
        where TValue : notnull
    {
        private readonly Dictionary<TValue, Occurrence> _data;

        // TODO: Remove logic from constructor
        internal OccurrenceAnalysis(OccurrenceCollection<TValue> occurrences)
        {
            _data = new Dictionary<TValue, Occurrence>(occurrences.Count);

            if (occurrences.Count == 0)
            {
                return;
            }

            Total = occurrences.Total;

            var rank = 0;
            var skippedRanks = 0;
            var previousCount = (ulong?)null;
            foreach (var occurrence in occurrences.OrderByDescending(x => x.Count))
            {
                if (previousCount == occurrence.Count)
                {
                    skippedRanks++;
                }
                else
                {
                    rank += 1 + skippedRanks;
                    skippedRanks = 0;
                }
                previousCount = occurrence.Count;

                _data[occurrence.Value] = new Occurrence(occurrence.Value, occurrence.Count, rank, occurrence.Count / (double)Total);
            }
        }

        public int Count => _data.Count;

        public ulong Total { get; }

        public Occurrence this[TValue value] => _data[value];

        public bool Contains(TValue value)
        {
            return _data.ContainsKey(value);
        }

        public bool TryGet(TValue value, [MaybeNullWhen(false)] out Occurrence occurrence)
        {
            return _data.TryGetValue(value, out occurrence);
        }

        public IEnumerator<Occurrence> GetEnumerator()
        {
            return _data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public record Occurrence(TValue Value, ulong Count, int Rank, double Percentage) : IOccurrenceItem<TValue>;
    }
}
