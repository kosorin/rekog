namespace Rekog.Core.Corpora
{
    public class CorpusAnalysis
    {
        public CorpusAnalysis(CorpusData data)
        {
            Unigrams = new OccurrenceAnalysis<string>(data.UnigramOccurrences);
            Bigrams = new OccurrenceAnalysis<string>(data.BigramOccurrences);
            Trigrams = new OccurrenceAnalysis<string>(data.TrigramOccurrences);
        }

        public OccurrenceAnalysis<string> Unigrams { get; }

        public OccurrenceAnalysis<string> Bigrams { get; }

        public OccurrenceAnalysis<string> Trigrams { get; }
    }
}
