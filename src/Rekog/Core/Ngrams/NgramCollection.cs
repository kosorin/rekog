using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Ngrams
{
    public class NgramCollection : IEnumerable<Ngram>
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

            var rank = 1;
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

                var percentage = rawNgram.Occurrences / (double)TotalOccurrences;

                _data[rawNgram.Value] = new Ngram(rawNgram.Value, rank, percentage, rawNgram.Occurrences);

                rank++;
            }
        }

        public int Size { get; }

        public ulong TotalOccurrences { get; }

        public Ngram this[string ngram] => _data[ngram];

        public IEnumerator<Ngram> GetEnumerator()
        {
            return _data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
