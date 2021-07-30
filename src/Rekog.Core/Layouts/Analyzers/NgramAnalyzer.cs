using System.Collections.Generic;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class NgramAnalyzer<T> : Analyzer, INgramAnalyzer
        where T : notnull
    {
        private readonly OccurrenceCollection<(T value, double? effort)> _occurrences = new OccurrenceCollection<(T value, double? effort)>();

        protected NgramAnalyzer(string description)
            : base(description)
        {
        }

        public abstract int Size { get; }

        public void Analyze(Occurrence<LayoutNgram> ngram)
        {
            if (TryGetValue(ngram.Value.Keys, out var value))
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

        public override LayoutAnalysisNode GetResult()
        {
            List<(T value, LayoutAnalysisNode node)> items = _occurrences.Analyze().Occurrences.Values
                .GroupBy(x => x.Value.value)
                .Select(g => (value: g.Key, node: new LayoutAnalysisNode(g.Key.ToString()!,
                    g.Sum(x => x.Percentage),
                    g.Any(x => x.Value.effort.HasValue) ? g.Sum(x => x.Percentage * x.Value.effort) : null)))
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

        protected abstract bool TryGetValue(Key[] keys, out (T value, double? effort) value);
    }
}
