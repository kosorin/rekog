using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class NgramAnalyzer<T> : Analyzer, INgramAnalyzer
        where T : notnull
    {
        private readonly OccurrenceCollection<LayoutNgramAnalysis<T>> _occurrences = new OccurrenceCollection<LayoutNgramAnalysis<T>>();

        protected NgramAnalyzer(string description)
            : base(description)
        {
        }

        public abstract int Size { get; }

        public void Analyze(Occurrence<LayoutNgram> ngram)
        {
            if (TryAnalyze(ngram.Value.Keys, out var result))
            {
                _occurrences.Add(result, ngram.Count);
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

        public override LayoutAnalysisNode GetResult()
        {
            List<(T value, LayoutAnalysisNode node)> items = _occurrences.Analyze().Occurrences.Values
                .GroupBy(x => x.Value.Value)
                .Select(g => (value: g.Key, node: new LayoutAnalysisNode(g.Key.ToString()!,
                    g.Sum(x => x.Percentage),
                    g.Any(x => x.Value.Effort.HasValue) ? g.Sum(x => x.Percentage * x.Value.Effort) : null)))
                .ToList();

            if (typeof(T) == typeof(bool))
            {
                var trueNode = items.Where(x => Equals(true, x.value)).Select(x => x.node).FirstOrDefault();
                return new LayoutAnalysisNode(Description, trueNode?.Percentage ?? 0, trueNode?.Effort);
            }
            return new LayoutAnalysisNode(Description, GroupChildren(items));
        }

        protected virtual List<LayoutAnalysisNode> GroupChildren(List<(T value, LayoutAnalysisNode node)> items)
        {
            return items.Select(x => x.node).ToList();
        }

        protected abstract bool TryAnalyze(Key[] keys, [MaybeNullWhen(false)] out LayoutNgramAnalysis<T> result);
    }
}
