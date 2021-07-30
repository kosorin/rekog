namespace Rekog.Core.Layouts
{
    public record LayoutNgramAnalysis<TValue>
        where TValue : notnull
    {
        public LayoutNgramAnalysis(TValue value)
        {
            Value = value;
            Effort = null;
        }

        public LayoutNgramAnalysis(TValue value, double effort)
        {
            Value = value;
            Effort = effort;
        }

        public TValue Value { get; }

        public double? Effort { get; }
    }
}
