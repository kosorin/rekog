using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rekog.Core
{
    public class OccurrenceCollection<TValue> : IOccurrenceMap<TValue, Occurrence<TValue>>
        where TValue : notnull
    {
        private readonly Dictionary<TValue, Occurrence<TValue>> _occurrences;

        public OccurrenceCollection()
        {
            _occurrences = new();
        }

        public OccurrenceCollection(IReadOnlyDictionary<TValue, ulong> occurrences)
        {
            ItemTotal = GetItemTotal(occurrences);
            NullTotal = 0;

            _occurrences = occurrences.ToDictionary(x => x.Key, x => new Occurrence<TValue>(x.Key, x.Value));
        }

        // TODO: Add unit test
        public OccurrenceCollection(IReadOnlyDictionary<TValue, ulong> occurrences, ulong total)
        {
            ItemTotal = GetItemTotal(occurrences);
            if (total < ItemTotal)
            {
                throw new ArgumentException(null, nameof(total));
            }
            NullTotal = total - ItemTotal;

            _occurrences = occurrences.ToDictionary(x => x.Key, x => new Occurrence<TValue>(x.Key, x.Value));
        }

        public int Count => _occurrences.Count;

        public ulong Total => NullTotal + ItemTotal;

        public ulong ItemTotal { get; private set; }

        public ulong NullTotal { get; private set; }

        public Occurrence<TValue> this[TValue value] => _occurrences[value];

        // TODO: Add unit test
        public OccurrenceAnalysis<TValue> Analyze()
        {
            return new(this);
        }

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
            ItemTotal += count;
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
        public OccurrenceCollection<TGroup> Group<TGroup>(Func<Occurrence<TValue>, TGroup> groupSelector)
            where TGroup : notnull
        {
            var groupedOccurrences = _occurrences.Values
                .GroupBy(groupSelector)
                .ToDictionary(g => g.Key, g => (ulong)g.Sum(x => (decimal)x.Count));
            return new OccurrenceCollection<TGroup>(groupedOccurrences, Total);
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

        private static ulong GetItemTotal(IReadOnlyDictionary<TValue, ulong> occurrences)
        {
            return (ulong)occurrences.Values.Sum(x => (decimal)x);
        }
    }
}
