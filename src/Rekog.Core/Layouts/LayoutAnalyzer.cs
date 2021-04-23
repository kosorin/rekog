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
                new FingerFrequencyAnalyzer(),

                new SameFingerBigramAnalyzer(),
                new NeighborFingerBigramAnalyzer(),

                new SameHandRollAnalyzer(),
                new SameHandLongRollAnalyzer(),

                new SameFingerMotionAnalyzer(),

                new RowFrequencyAnalyzer(),
                new SameHandJumpAnalyzer(),
                new HandAlternationAnalyzer(),
                new HomingAnalyzer(),

                new LayerSwitchAnalyzer(),
            };

            var unigramAnalyzers = ngramAnalyzers.Where(x => x.Size == 1).ToArray();
            var unigrams = GetNgramKeyOccurrences2(corpusAnalysisData.UnigramOccurrences, layout);
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
            var bigrams = GetNgramKeyOccurrences2(corpusAnalysisData.BigramOccurrences, layout);
            foreach (var unigram in bigrams)
            {
                foreach (var analyzer in bigramAnalyzers)
                {
                    analyzer.Analyze(unigram);
                }
            }
            foreach (var analyzer in bigramAnalyzers)
            {
                analyzer.AnalyzeNull(bigrams.NullTotal);
            }

            var trigramAnalyzers = ngramAnalyzers.Where(x => x.Size == 3).ToArray();
            var trigrams = GetNgramKeyOccurrences2(corpusAnalysisData.TrigramOccurrences, layout);
            foreach (var unigram in trigrams)
            {
                foreach (var analyzer in trigramAnalyzers)
                {
                    analyzer.Analyze(unigram);
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

        private OccurrenceCollection<LayoutNgram> GetNgramKeyOccurrences2(OccurrenceCollection<string> ngramOccurrences, Layout layout)
        {
            var unigramsData = ngramOccurrences.ToDictionary(x => new LayoutNgram(x.Value, layout.GetNgramKeys(x.Value)), x => x.Count);
            return new OccurrenceCollection<LayoutNgram>(unigramsData, ngramOccurrences.Total);
        }
    }
}
