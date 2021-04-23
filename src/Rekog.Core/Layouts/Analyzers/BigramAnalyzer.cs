namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class BigramAnalyzer<T> : NgramAnalyzer<T>
        where T : notnull
    {
        protected BigramAnalyzer(string description)
            : base(description)
        {
        }

        public override int Size => 2;

        protected override bool TryGetValue(Key[] keys, out (T, double?) value)
        {
            return TryGetValue(keys[0], keys[1], out value);
        }

        protected abstract bool TryGetValue(Key firstKey, Key secondKey, out (T, double?) value);
    }
}
