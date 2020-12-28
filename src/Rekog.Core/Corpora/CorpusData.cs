using System;

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
    }
}
