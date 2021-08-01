using System.Collections;

namespace Rekog.App.ObjectModel
{
    public class CollectionItemChangedEventArgs
    {
        public CollectionItemChangedEventArgs(ICollection oldItems, ICollection newItems)
        {
            OldItems = oldItems;
            NewItems = newItems;
        }

        public ICollection OldItems { get; }

        public ICollection NewItems { get; }
    }
}
