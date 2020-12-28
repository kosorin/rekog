namespace Rekog.Core
{
    public interface IOccurrenceItem<TValue>
        where TValue : notnull
    {
        TValue Value { get; }

        ulong Count { get; }
    }
}
