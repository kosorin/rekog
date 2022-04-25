using Rekog.App.ObjectModel;

namespace Rekog.App.Extensions
{
    public static class IObservableObjectCollectionExtensions
    {
        public static void Subscribe<TCollection, TItem>(this TCollection collection,
            CollectionItemChangedEventHandler<TItem>? onCollectionItemChanged,
            CollectionItemPropertyChangedEventHandler<TItem>? onCollectionItemPropertyChanged)
            where TCollection : IObservableObjectCollection<TItem>
            where TItem : ObservableObject
        {
            Unsubscribe(collection, onCollectionItemChanged, onCollectionItemPropertyChanged);

            if (onCollectionItemChanged != null)
            {
                collection.CollectionItemChanged += onCollectionItemChanged;
            }
            if (onCollectionItemPropertyChanged != null)
            {
                collection.CollectionItemPropertyChanged += onCollectionItemPropertyChanged;
            }
        }

        public static void Unsubscribe<TCollection, TItem>(this TCollection collection,
            CollectionItemChangedEventHandler<TItem>? onCollectionItemChanged,
            CollectionItemPropertyChangedEventHandler<TItem>? onCollectionItemPropertyChanged)
            where TCollection : IObservableObjectCollection<TItem>
            where TItem : ObservableObject
        {
            if (onCollectionItemChanged != null)
            {
                collection.CollectionItemChanged -= onCollectionItemChanged;
            }
            if (onCollectionItemPropertyChanged != null)
            {
                collection.CollectionItemPropertyChanged -= onCollectionItemPropertyChanged;
            }
        }
    }
}
