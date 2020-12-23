using Rekog.Core.Corpora;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    public abstract class BigramLayoutAnalyzer<T> : OccurrenceLayoutAnalyzer<T>
        where T : notnull
    {
        protected BigramLayoutAnalyzer(string description) : base(description)
        {
        }

        public override void Analyze(CorpusAnalysis corpusAnalysis, Layout layout)
        {
            foreach (var unigram in corpusAnalysis.Unigrams)
            {
                if (layout.TryGetKey(unigram.Value[0], out var firstKey) && layout.TryGetKey(unigram.Value[0], out var secondKey))
                {
                    if (TryGet(firstKey, secondKey, out var value))
                    {
                        Occurrences.Add(value, unigram.Count);
                        continue;
                    }
                }

                Occurrences.AddNull(unigram.Count);
            }
        }

        protected abstract bool TryGet(Key firstKey, Key secondKey, [MaybeNullWhen(false)] out T value);
    }
}
