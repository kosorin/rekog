using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class BigramAnalyzer<T> : NgramAnalyzer<T>
        where T : notnull
    {
        protected BigramAnalyzer(string description)
            : base(description)
        {
        }

        public sealed override int Size => 2;

        protected sealed override bool TryAnalyze(Key[] keys, [MaybeNullWhen(false)] out LayoutNgramAnalysis<T> result)
        {
            return TryAnalyze(keys[0], keys[1], out result);
        }

        protected abstract bool TryAnalyze(Key firstKey, Key secondKey, [MaybeNullWhen(false)] out LayoutNgramAnalysis<T> result);
    }
}
