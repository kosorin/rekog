using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rekog.Core
{
    public class OccurrenceCollection<TValue> : IReadOnlyCollection<Occurrence<TValue>>
        where TValue : notnull
    {
        private readonly Dictionary<TValue, Occurrence<TValue>> _occurrences;

        public OccurrenceCollection()
        {
            _occurrences = new();
        }

        public OccurrenceCollection(IReadOnlyDictionary<TValue, ulong> occurrences)
        {
            _occurrences = occurrences.ToDictionary(x => x.Key, x => new Occurrence<TValue>(x.Key, x.Value));

            ValueTotal = (ulong)occurrences.Values.Sum(x => (decimal)x);
            NullTotal = 0;
        }

        // TODO: Add unit test
        public OccurrenceCollection(IReadOnlyDictionary<TValue, ulong> occurrences, ulong total)
            : this(occurrences)
        {
            if (total < ValueTotal)
            {
                throw new ArgumentException(null, nameof(total));
            }
            NullTotal = total - ValueTotal;
        }

        public int Count => _occurrences.Count;

        public ulong Total => ValueTotal + NullTotal;

        public ulong ValueTotal { get; private set; }

        public ulong NullTotal { get; private set; }

        public Occurrence<TValue> this[TValue value] => _occurrences[value];

        public void AddNull()
        {
            AddNull(1);
        }

        public void AddNull(ulong count)
        {
            NullTotal += count;
        }

        public void Add(TValue value)
        {
            Add(value, 1);
        }

        public void Add(TValue value, ulong count)
        {
            if (!_occurrences.TryGetValue(value, out var occurrence))
            {
                _occurrences.Add(value, occurrence = new Occurrence<TValue>(value));
            }
            occurrence.Add(count);
            ValueTotal += count;
        }

        public void Add(OccurrenceCollection<TValue> occurrences)
        {
            if (occurrences == this)
            {
                throw new ArgumentException(null, nameof(occurrences));
            }

            foreach (var occurrence in occurrences)
            {
                Add(occurrence.Value, occurrence.Count);
            }
        }

        // TODO: Add unit test
        public OccurrenceCollection<TGroupValue> Group<TGroupValue>(Func<Occurrence<TValue>, TGroupValue> groupValueSelector)
            where TGroupValue : notnull
        {
            var groupedOccurrences = _occurrences.Values
                .GroupBy(groupValueSelector)
                .ToDictionary(g => g.Key, g => (ulong)g.Sum(x => (decimal)x.Count));
            return new OccurrenceCollection<TGroupValue>(groupedOccurrences, Total);
        }

        // TODO: Add unit test
        public OccurrenceCollectionAnalysis<TValue> Analyze()
        {
            var occurrences = new Dictionary<TValue, OccurrenceAnalysis<TValue>>(_occurrences.Count);

            var rank = 0;
            var skippedRanks = 0;
            var previousCount = (ulong?)null;
            foreach (var occurrence in _occurrences.Values.OrderByDescending(x => x.Count))
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

                occurrences[occurrence.Value] = new OccurrenceAnalysis<TValue>(occurrence.Value, occurrence.Count, rank, occurrence.Count / (double)Total);
            }

            return new OccurrenceCollectionAnalysis<TValue>(occurrences, Total, ValueTotal, NullTotal);
        }

        public bool Contains(TValue value)
        {
            return _occurrences.ContainsKey(value);
        }

        public bool TryGet(TValue value, [MaybeNullWhen(false)] out Occurrence<TValue> occurrence)
        {
            return _occurrences.TryGetValue(value, out occurrence);
        }

        public IEnumerator<Occurrence<TValue>> GetEnumerator()
        {
            return _occurrences.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
