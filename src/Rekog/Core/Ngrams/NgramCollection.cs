using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rekog.Core.Ngrams
{
    public class NgramCollection : IReadOnlyDictionary<string, NgramCollection.Ngram>
    {
        private readonly Dictionary<string, Ngram> _data;

        public NgramCollection()
        {
            _data = new Dictionary<string, Ngram>();
        }

        public NgramCollection(ICollection<RawNgram> rawNgrams)
        {
            _data = new Dictionary<string, Ngram>(rawNgrams.Count);

            if (rawNgrams.Count == 0)
            {
                return;
            }

            Size = rawNgrams.First().Value.Length;
            TotalOccurrences = (ulong)rawNgrams.Sum(x => (decimal)x.Occurrences);

            var rank = 0;
            var skippedRanks = 0;
            var previousOccurrences = (ulong?)null;
            foreach (var rawNgram in rawNgrams.OrderByDescending(x => x.Occurrences))
            {
                if (_data.ContainsKey(rawNgram.Value))
                {
                    throw new ArgumentException($"Ngram '{rawNgram.Value}' was specified multiple times.", nameof(rawNgrams));
                }
                if (Size != rawNgram.Value.Length)
                {
                    throw new FormatException($"All ngrams must be of same length '{Size}'.");
                }

                if (previousOccurrences == rawNgram.Occurrences)
                {
                    skippedRanks++;
                }
                else
                {
                    rank += 1 + skippedRanks;
                    skippedRanks = 0;
                }
                previousOccurrences = rawNgram.Occurrences;

                var percentage = rawNgram.Occurrences / (double)TotalOccurrences;

                _data[rawNgram.Value] = new Ngram(rawNgram.Value, rank, percentage, rawNgram.Occurrences);
            }
        }

        public int Size { get; }

        public ulong TotalOccurrences { get; }

        public int Count => _data.Count;

        public IEnumerable<string> Keys => _data.Keys;

        public IEnumerable<Ngram> Values => _data.Values;

        public Ngram this[string ngramValue] => _data[ngramValue];

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out Ngram value)
        {
            return _data.TryGetValue(key, out value);
        }

        public List<RawNgram> ToRawNgrams()
        {
            return Values.Select(x => new RawNgram(x.Value, x.Occurrences)).ToList();
        }

        public IEnumerator<KeyValuePair<string, Ngram>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.Values.GetEnumerator();
        }

        public sealed record Ngram(string Value, int Rank, double Percentage, ulong Occurrences);
    }
}
