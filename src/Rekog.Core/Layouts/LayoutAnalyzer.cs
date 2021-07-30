using System.Linq;
using Rekog.Core.Corpora;
using Rekog.Core.Layouts.Analyzers;

namespace Rekog.Core.Layouts
{
    public class LayoutAnalyzer
    {
        public LayoutAnalysisNode Analyze(CorpusAnalysisData corpusAnalysisData, Layout layout)
        {
            var ngramAnalyzers = new INgramAnalyzer[]
            {
                // Ngram
                new SameFingerBigramAnalyzer(),
                new NeighborFingerBigramAnalyzer(),

                // Roll
                new SameHandRollAnalyzer(),
                new SameHandLongRollAnalyzer(),

                // Motion
                new SameFingerMotionAnalyzer(),

                // Row
                new RowFrequencyAnalyzer(),

                // Hand
                new HandAlternationAnalyzer(),

                // Finger
                new FingerFrequencyAnalyzer(),

                // Other
                new HomingAnalyzer(),
                new LayerSwitchAnalyzer(),
            };

            var unigramAnalyzers = ngramAnalyzers.Where(x => x.Size == 1).ToArray();
            var unigrams = layout.GetNgramOccurrences(corpusAnalysisData.Unigrams);
            foreach (var unigram in unigrams)
            {
                foreach (var analyzer in unigramAnalyzers)
                {
                    analyzer.Analyze(unigram);
                }
            }
            foreach (var analyzer in unigramAnalyzers)
            {
                analyzer.AnalyzeNull(unigrams.NullTotal);
            }

            var bigramAnalyzers = ngramAnalyzers.Where(x => x.Size == 2).ToArray();
            var bigrams = layout.GetNgramOccurrences(corpusAnalysisData.Bigrams);
            foreach (var bigram in bigrams)
            {
                foreach (var analyzer in bigramAnalyzers)
                {
                    analyzer.Analyze(bigram);
                }
            }
            foreach (var analyzer in bigramAnalyzers)
            {
                analyzer.AnalyzeNull(bigrams.NullTotal);
            }

            var trigramAnalyzers = ngramAnalyzers.Where(x => x.Size == 3).ToArray();
            var trigrams = layout.GetNgramOccurrences(corpusAnalysisData.Trigrams);
            foreach (var trigram in trigrams)
            {
                foreach (var analyzer in trigramAnalyzers)
                {
                    analyzer.Analyze(trigram);
                }
            }
            foreach (var analyzer in trigramAnalyzers)
            {
                analyzer.AnalyzeNull(trigrams.NullTotal);
            }

            return new LayoutAnalysisNode("Result", ngramAnalyzers.Select(x => x.GetResult()).ToList());
        }
    }
}
