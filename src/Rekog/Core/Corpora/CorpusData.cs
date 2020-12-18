using Rekog.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Corpora
{
    public class CorpusData
    {
        public CorpusData()
        {
            UnigramOccurrences = new();
            BigramOccurrences = new();
            TrigramOccurrences = new();
        }

        public CorpusData(OccurrenceCollection<string> unigramOccurrences, OccurrenceCollection<string> bigramOccurrences, OccurrenceCollection<string> trigramOccurrences)
        {
            UnigramOccurrences = unigramOccurrences;
            BigramOccurrences = bigramOccurrences;
            TrigramOccurrences = trigramOccurrences;
        }

        public CorpusData(CorpusReport report)
        {
            UnigramOccurrences = new(report.Unigrams);
            BigramOccurrences = new(report.Bigrams);
            TrigramOccurrences = new(report.Trigrams);
        }

        public OccurrenceCollection<string> UnigramOccurrences { get; }

        public OccurrenceCollection<string> BigramOccurrences { get; }

        public OccurrenceCollection<string> TrigramOccurrences { get; }

        public void Add(CorpusData other)
        {
            if (other == this)
            {
                throw new ArgumentException(null, nameof(other));
            }

            UnigramOccurrences.Add(other.UnigramOccurrences);
            BigramOccurrences.Add(other.BigramOccurrences);
            TrigramOccurrences.Add(other.TrigramOccurrences);
        }

        public CorpusReport ToReport()
        {
            return new CorpusReport
            {
                Unigrams = GetRawCollection(UnigramOccurrences),
                Bigrams = GetRawCollection(BigramOccurrences),
                Trigrams = GetRawCollection(TrigramOccurrences),
            };

            static Dictionary<string, ulong> GetRawCollection(OccurrenceCollection<string> ngramOccurrences)
            {
                return ngramOccurrences.OrderByDescending(x => x.Count).ToDictionary(x => x.Value, x => x.Count);
            }
        }
    }
}
