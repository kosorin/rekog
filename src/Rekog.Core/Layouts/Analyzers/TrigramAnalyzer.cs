using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class TrigramAnalyzer<T> : NgramAnalyzer<T>
        where T : notnull
    {
        protected TrigramAnalyzer(string description)
            : base(description)
        {
        }

        public sealed override int Size => 3;

        protected sealed override bool TryAnalyze(Key[] keys, [MaybeNullWhen(false)] out LayoutNgramAnalysis<T> result)
        {
            return TryAnalyze(keys[0], keys[1], keys[2], out result);
        }

        protected abstract bool TryAnalyze(Key firstKey, Key secondKey, Key thirdKey, [MaybeNullWhen(false)] out LayoutNgramAnalysis<T> result);
    }
}
