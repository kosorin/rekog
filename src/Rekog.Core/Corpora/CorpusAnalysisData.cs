using System.Text;

namespace Rekog.Core.Corpora
{
    public class CorpusAnalysisData
    {
        public CorpusAnalysisData()
        {
            Unigrams = new OccurrenceCollection<string>();
            Bigrams = new OccurrenceCollection<string>();
            Trigrams = new OccurrenceCollection<string>();
            ReplacedCharacters = new OccurrenceCollection<Rune>();
            SkippedCharacters = new OccurrenceCollection<Rune>();
        }

        public CorpusAnalysisData(OccurrenceCollection<string> unigrams, OccurrenceCollection<string> bigrams, OccurrenceCollection<string> trigrams, OccurrenceCollection<Rune> replacedCharacters, OccurrenceCollection<Rune> skippedCharacters)
        {
            Unigrams = unigrams;
            Bigrams = bigrams;
            Trigrams = trigrams;
            ReplacedCharacters = replacedCharacters;
            SkippedCharacters = skippedCharacters;
        }

        public OccurrenceCollection<string> Unigrams { get; }

        public OccurrenceCollection<string> Bigrams { get; }

        public OccurrenceCollection<string> Trigrams { get; }

        public OccurrenceCollection<Rune> ReplacedCharacters { get; }

        public OccurrenceCollection<Rune> SkippedCharacters { get; }

        public void Add(CorpusAnalysisData other)
        {
            Unigrams.Add(other.Unigrams);
            Bigrams.Add(other.Bigrams);
            Trigrams.Add(other.Trigrams);
            ReplacedCharacters.Add(other.ReplacedCharacters);
            SkippedCharacters.Add(other.SkippedCharacters);
        }
    }
}
