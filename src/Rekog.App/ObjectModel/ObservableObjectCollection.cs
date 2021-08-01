using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Rekog.App.ObjectModel
{
    public class ObservableObjectCollection<T> : ObservableCollection<T>, IObservableObjectCollection
        where T : ObservableObject
    {
        public ObservableObjectCollection()
        {
        }

        public ObservableObjectCollection(IEnumerable<T> collection) : base(collection)
        {
            Subscribe(Items);
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            Subscribe(item);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = Items[index];
            if (ReferenceEquals(oldItem, item))
            {
                return;
            }

            Unsubscribe(oldItem);
            base.SetItem(index, item);
            Subscribe(item);
        }

        protected override void RemoveItem(int index)
        {
            Unsubscribe(Items[index]);
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            Unsubscribe(Items);
            base.ClearItems();
        }

        protected virtual void OnCollectionItemPropertyChanged(object item, CollectionItemPropertyChangedEventArgs args)
        {
            CollectionItemPropertyChanged?.Invoke(item, args);
        }

        protected virtual void OnCollectionItemChanged(CollectionItemChangedEventArgs args)
        {
            CollectionItemChanged?.Invoke(this, args);
        }

        private void Subscribe(T item)
        {
            Subscribe(new[] { item, });
        }

        private void Unsubscribe(T item)
        {
            Unsubscribe(new[] { item, });
        }

        private void Subscribe(IEnumerable<T> items)
        {
            var newItems = items.ToArray();

            OnCollectionItemChanged(new CollectionItemChangedEventArgs(Array.Empty<T>(), newItems));
            foreach (var item in newItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void Unsubscribe(IEnumerable<T> items)
        {
            var oldItems = items.ToArray();

            foreach (var item in oldItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            OnCollectionItemChanged(new CollectionItemChangedEventArgs(oldItems, Array.Empty<T>()));
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (sender is T item)
            {
                OnCollectionItemPropertyChanged(item, new CollectionItemPropertyChangedEventArgs(args.PropertyName));
            }
        }

        public event CollectionItemChangedEventHandler? CollectionItemChanged;

        public event CollectionItemPropertyChangedEventHandler? CollectionItemPropertyChanged;
    }
}
