using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Core
{
    public interface IOccurrenceMap<TValue, TItem> : IReadOnlyCollection<TItem>
        where TValue : notnull
        where TItem : notnull, IOccurrenceItem<TValue>
    {
        ulong Total { get; }

        TItem this[TValue value] { get; }

        bool Contains(TValue value);

        bool TryGet(TValue value, [MaybeNullWhen(false)] out TItem item);
    }
}
