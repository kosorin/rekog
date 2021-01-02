namespace Rekog.Core.Layouts.Analyzers
{
    internal abstract class UnigramAnalyzer<T> : NgramAnalyzer<T>
        where T : notnull
    {
        protected UnigramAnalyzer(string description) : base(description)
        {
        }

        public override int Size => 1;

        protected override bool TryGetValue(Key[] keys, out (T, double?) value)
        {
            return TryGetValue(keys[0], out value);
        }

        protected abstract bool TryGetValue(Key key, out (T, double?) value);
    }
}
