using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class NgramAnalyzer<T> : Analyzer, INgramAnalyzer
        where T : notnull
    {
        private readonly OccurrenceCollection<(T value, double? effort)> _occurrences = new();

        protected NgramAnalyzer(string description) : base(description)
        {
        }

        public abstract int Size { get; }

        public void Analyze(Occurrence<LayoutNgram> ngram)
        {
            if (ngram.Value.Keys != null && TryGetValue(ngram.Value.Keys, out var value))
            {
                _occurrences.Add(value, ngram.Count);
            }
            else
            {
                AnalyzeNull(ngram.Count);
            }
        }

        public void AnalyzeNull(ulong count)
        {
            _occurrences.AddNull(count);
        }

        public override LayoutAnalysisResult GetResult()
        {
            var items = _occurrences.Analyze().Occurrences.Values
                .GroupBy(x => x.Value.value)
                .Select(g => (g.Key, new LayoutAnalysisResult(g.Key.ToString()!)
                {
                    Percentage = g.Sum(x => x.Percentage),
                    Effort = g.Any(x => x.Value.effort.HasValue) ? g.Sum(x => x.Percentage * x.Value.effort) : null,
                }))
                .ToList();

            if (items is List<(bool value, LayoutAnalysisResult result)> boolItems)
            {
                var trueResult = boolItems.Where(x => x.value).Select(x => x.result).FirstOrDefault();
                return new LayoutAnalysisResult(Description)
                {
                    Effort = trueResult?.Effort,
                    Percentage = trueResult?.Percentage ?? 0,
                };
            }
            return new LayoutAnalysisResult(Description, GroupResultItems(items));
        }

        protected virtual List<LayoutAnalysisResult> GroupResultItems(List<(T value, LayoutAnalysisResult result)> items)
        {
            return items.Select(x => x.result).ToList();
        }

        protected abstract bool TryGetValue(Key[] keys, out (T value, double? effort) value);
    }
}
