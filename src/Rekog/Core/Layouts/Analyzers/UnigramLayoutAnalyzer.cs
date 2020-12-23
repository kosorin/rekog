using Rekog.Core.Corpora;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    public abstract class UnigramLayoutAnalyzer<T> : OccurrenceLayoutAnalyzer<T>
        where T : notnull
    {
        protected UnigramLayoutAnalyzer(string description) : base(description)
        {
        }

        public override void Analyze(CorpusAnalysis corpusAnalysis, Layout layout)
        {
            foreach (var unigram in corpusAnalysis.Unigrams)
            {
                if (layout.TryGetKey(unigram.Value[0], out var key))
                {
                    if (TryGet(key, out var value))
                    {
                        Occurrences.Add(value, unigram.Count);
                        continue;
                    }
                }

                Occurrences.AddNull(unigram.Count);
            }
        }

        protected abstract bool TryGet(Key key, [MaybeNullWhen(false)] out T value);
    }
}
