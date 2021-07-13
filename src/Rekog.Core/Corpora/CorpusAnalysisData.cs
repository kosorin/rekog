namespace Rekog.Core.Corpora
{
    public class CorpusAnalysisData
    {
        public CorpusAnalysisData()
        {
            UnigramOccurrences = new OccurrenceCollection<string>();
            BigramOccurrences = new OccurrenceCollection<string>();
            TrigramOccurrences = new OccurrenceCollection<string>();
        }

        public CorpusAnalysisData(OccurrenceCollection<string> unigramOccurrences, OccurrenceCollection<string> bigramOccurrences, OccurrenceCollection<string> trigramOccurrences)
        {
            UnigramOccurrences = unigramOccurrences;
            BigramOccurrences = bigramOccurrences;
            TrigramOccurrences = trigramOccurrences;
        }

        public OccurrenceCollection<string> UnigramOccurrences { get; }

        public OccurrenceCollection<string> BigramOccurrences { get; }

        public OccurrenceCollection<string> TrigramOccurrences { get; }

        public void Add(CorpusAnalysisData other)
        {
            UnigramOccurrences.Add(other.UnigramOccurrences);
            BigramOccurrences.Add(other.BigramOccurrences);
            TrigramOccurrences.Add(other.TrigramOccurrences);
        }
    }
}
