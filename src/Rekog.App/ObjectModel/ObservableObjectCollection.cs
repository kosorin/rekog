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
            Subscribe(Items.ToArray());
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            Subscribe(new[] { item, });
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = Items[index];
            if (ReferenceEquals(oldItem, item))
            {
                return;
            }

            var oldItems = new[] { oldItem, };
            BeginUnsubscribe(oldItems);
            base.SetItem(index, item);
            EndUnsubscribe(oldItems);
            Subscribe(new[] { item, });
        }

        protected override void RemoveItem(int index)
        {
            var oldItems = new[] { Items[index], };
            BeginUnsubscribe(oldItems);
            base.RemoveItem(index);
            EndUnsubscribe(oldItems);
        }

        protected override void ClearItems()
        {
            var oldItems = Items.ToArray();
            BeginUnsubscribe(oldItems);
            base.ClearItems();
            EndUnsubscribe(oldItems);
        }

        protected virtual void OnCollectionItemPropertyChanged(object item, CollectionItemPropertyChangedEventArgs args)
        {
            CollectionItemPropertyChanged?.Invoke(item, args);
        }

        protected virtual void OnCollectionItemChanged(CollectionItemChangedEventArgs args)
        {
            CollectionItemChanged?.Invoke(this, args);
        }

        private void Subscribe(T[] newItems)
        {
            OnCollectionItemChanged(new CollectionItemChangedEventArgs(Array.Empty<T>(), newItems));
            foreach (var item in newItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void BeginUnsubscribe(T[] oldItems)
        {
            foreach (var item in oldItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void EndUnsubscribe(T[] oldItems)
        {
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
