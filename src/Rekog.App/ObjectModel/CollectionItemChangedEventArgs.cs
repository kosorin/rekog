using System.Collections.Generic;

namespace Rekog.App.ObjectModel
{
    public class CollectionItemChangedEventArgs<T>
        where T : ObservableObject
    {
        public CollectionItemChangedEventArgs(IReadOnlyCollection<T> oldItems, IReadOnlyCollection<T> newItems)
        {
            OldItems = oldItems;
            NewItems = newItems;
        }

        public IReadOnlyCollection<T> OldItems { get; }

        public IReadOnlyCollection<T> NewItems { get; }
    }
}
