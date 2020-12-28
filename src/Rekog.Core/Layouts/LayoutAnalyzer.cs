using Rekog.Core.Corpora;
using Rekog.Core.Layouts.Analyzers;

namespace Rekog.Core.Layouts
{
    public class LayoutAnalyzer
    {
        public void Analyze(CorpusAnalysis corpusAnalysis, Layout layout)
        {
            var unigramAnalyzers = new IUnigramAnalyzer[]
            {
                new FingerFrequencyAnalyzer(),
                new HandFrequencyAnalyzer(),
                new RowFrequencyAnalyzer(),
            };
            foreach (var unigram in corpusAnalysis.Unigrams)
            {
                if (layout.TryGetKey(unigram.Value[0], out var key))
                {
                    foreach (var analyzer in unigramAnalyzers)
                    {
                        analyzer.Analyze(key, unigram.Count);
                    }
                }
                else
                {
                    foreach (var analyzer in unigramAnalyzers)
                    {
                        analyzer.Skip(unigram.Count);
                    }
                }
            }

            var bigramAnalyzers = new IBigramAnalyzer[]
            {
                new HandAlternationAnalyzer(),
            };
            foreach (var bigram in corpusAnalysis.Bigrams)
            {
                if (layout.TryGetKey(bigram.Value[0], out var firstKey) && layout.TryGetKey(bigram.Value[1], out var secondKey))
                {
                    foreach (var analyzer in bigramAnalyzers)
                    {
                        analyzer.Analyze(firstKey, secondKey, bigram.Count);
                    }
                }
                else
                {
                    foreach (var analyzer in bigramAnalyzers)
                    {
                        analyzer.Skip(bigram.Count);
                    }
                }
            }
        }

    }
}
