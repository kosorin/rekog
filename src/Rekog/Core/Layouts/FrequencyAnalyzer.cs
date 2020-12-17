using Rekog.Core.Corpora;
using Rekog.Core.Extensions;

namespace Rekog.Core.Layouts
{
    public class FrequencyAnalyzer
    {
        public void Analyze(CorpusAnalysis corpusAnalysis, Layout layout)
        {
            var fingerOccurrences = new OccurrenceCollection<Finger>();
            var handOccurrences = new OccurrenceCollection<Hand>();
            var rowOccurrences = new OccurrenceCollection<int>();

            foreach (var ngram in corpusAnalysis.Unigrams)
            {
                var character = ngram.Value[0];

                var finger = layout.GetFinger(character);
                fingerOccurrences.Add(finger, ngram.Count);

                var hand = layout.GetHand(character);
                handOccurrences.Add(hand, ngram.Count);

                var row = layout.GetRow(character);
                rowOccurrences.Add(row, ngram.Count);
            }
        }
    }

    public class NgramAnalyzer
    {
        public void Analyze(CorpusAnalysis corpusAnalysis, Layout layout)
        {
            var sameFingerBigramOccurrences = new OccurrenceCollection<Finger>();
            var neighborFingerBigramOccurrences = new OccurrenceCollection<(Finger, Finger)>();

            foreach (var ngram in corpusAnalysis.Bigrams)
            {
                var firstCharacter = ngram.Value[0];
                var secondCharacter = ngram.Value[1];

                var firstFinger = layout.GetFinger(firstCharacter);
                var secondFinger = layout.GetFinger(secondCharacter);
                if (firstFinger > secondFinger)
                {
                    (firstFinger, secondFinger) = (secondFinger, firstFinger);
                }

                if (firstFinger == secondFinger)
                {
                    sameFingerBigramOccurrences.Add(firstFinger, ngram.Count);
                }
                else
                {
                    sameFingerBigramOccurrences.AddTotal(ngram.Count);
                }

                if (firstFinger.IsNeighbor(secondFinger))
                {
                    neighborFingerBigramOccurrences.Add((firstFinger, secondFinger), ngram.Count);
                }
                else
                {
                    neighborFingerBigramOccurrences.AddTotal(ngram.Count);
                }
            }
        }
    }
}
