using System;
using System.Linq;
using System.Text;
using Rekog.Core.Corpora;
using Rekog.Core.Layouts.Analyzers;

namespace Rekog.Core.Layouts
{
    public class LayoutAnalyzer
    {
        public void Analyze(CorpusAnalysisData corpusAnalysisData, Layout layout)
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
                new SameHandJumpAnalyzer(),

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

            var result = new LayoutAnalysisResult("Result", ngramAnalyzers.Select(x => x.GetResult()).ToList());

            Print(result, 0);

            static void Print(LayoutAnalysisResult result, int indent)
            {
                const int maxDescriptionLength = 40;

                var sb = new StringBuilder(new string(' ', indent));
                var description = string.Concat(result.Description.Take(maxDescriptionLength - indent));
                sb.Append(description);
                sb.Append(new string(' ', maxDescriptionLength - indent - description.Length));
                sb.Append($"  {(indent > 0 ? result.GetTotalPercentage() : null),10:P3}");
                sb.Append($"  {result.GetTotalEffort(),10:N5}");
                Console.WriteLine(sb);

                var items = result.Items.AsEnumerable();
                if (indent == 0)
                {
                    items = items.OrderByDescending(x => x.Effort).ThenByDescending(x => x.Percentage).ThenByDescending(x => x.Description);
                }
                foreach (var item in items)
                {
                    Print(item, indent + 2);
                }
            }
        }
    }
}
