using System.Collections.Generic;

namespace Rekog.App.ObjectModel
{
    public class ListChangedEventArgs<TItem>
    {
        public ListChangedEventArgs(IReadOnlyList<TItem> newItems, IReadOnlyList<TItem> oldItems)
        {
            NewItems = newItems;
            OldItems = oldItems;
        }

        public IReadOnlyList<TItem> NewItems { get; }

        public IReadOnlyList<TItem> OldItems { get; }
    }
}
