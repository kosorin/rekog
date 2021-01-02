namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class TrigramAnalyzer<T> : NgramAnalyzer<T>
        where T : notnull
    {
        protected TrigramAnalyzer(string description) : base(description)
        {
        }

        public override int Size => 3;

        protected override bool TryGetValue(Key[] keys, out (T, double?) value)
        {
            return TryGetValue(keys[0], keys[1], keys[2], out value);
        }

        protected abstract bool TryGetValue(Key firstKey, Key secondKey, Key thirdKey, out (T, double?) value);
    }
}
