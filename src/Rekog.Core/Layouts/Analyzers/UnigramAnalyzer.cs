using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class UnigramAnalyzer<T> : NgramAnalyzer<T>
        where T : notnull
    {
        protected UnigramAnalyzer(string description)
            : base(description)
        {
        }

        public sealed override int Size => 1;

        protected sealed override bool TryAnalyze(Key[] keys, [MaybeNullWhen(false)] out LayoutNgramAnalysis<T> result)
        {
            return TryAnalyze(keys[0], out result);
        }

        protected abstract bool TryAnalyze(Key key, [MaybeNullWhen(false)] out LayoutNgramAnalysis<T> result);
    }
}
