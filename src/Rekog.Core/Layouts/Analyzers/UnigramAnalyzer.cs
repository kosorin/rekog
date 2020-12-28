using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class UnigramAnalyzer<T> : OccurrenceAnalyzer<T>, IUnigramAnalyzer
        where T : notnull
    {
        protected UnigramAnalyzer(string description) : base(description)
        {
        }

        public void Analyze(Key key, ulong count)
        {
            if (TryAccept(key, out var value))
            {
                Occurrences.Add(value, count);
            }
            else
            {
                Skip(count);
            }
        }

        protected abstract bool TryAccept(Key key, [MaybeNullWhen(false)] out T value);
    }
}
