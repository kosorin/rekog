using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class BigramAnalyzer<T> : OccurrenceAnalyzer<T>, IBigramAnalyzer
        where T : notnull
    {
        protected BigramAnalyzer(string description) : base(description)
        {
        }

        public void Analyze(Key firstKey, Key secondKey, ulong count)
        {
            if (TryAccept(firstKey, secondKey, out var value))
            {
                Occurrences.Add(value, count);
            }
            else
            {
                Skip(count);
            }
        }

        protected abstract bool TryAccept(Key firstKey, Key secondKey, [MaybeNullWhen(false)] out T value);
    }
}
