using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Rekog.App.ObjectModel
{
    [Obsolete("Use ObservableList instead")]
    public class ObservableObjectCollection<T> : ObservableCollection<T>, IObservableObjectCollection<T>
        where T : ObservableObject
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly IComparer<int> DescendingIndexComparer = Comparer<int>.Create((a, b) => b.CompareTo(a));

        public ObservableObjectCollection() : this(Array.Empty<T>())
        {
        }

        public ObservableObjectCollection(IEnumerable<T> collection) : base(collection)
        {
            Subscribe(Items.ToArray());
        }

        public void ReplaceUsingClear(IEnumerable<T> items)
        {
            var newItems = items.ToArray();

            if (newItems.Length == 0 && Count == 0)
            {
                return;
            }

            var oldItems = Items.ToArray();

            BeginSubscribe(oldItems);

            // Remove old
            base.ClearItems();

            // Add new
            var newItemIndex = Count;
            foreach (var newItem in newItems)
            {
                base.InsertItem(newItemIndex, newItem);
                newItemIndex++;
            }

            EndSubscribe(oldItems, newItems);
        }

        public void ReplaceUsingMerge(IEnumerable<T> items)
        {
            items = items.ToList();

            var oldItemsData = new SortedList<int, T>(DescendingIndexComparer);
            foreach (var oldItem in Items.Except(items))
            {
                if (IndexOf(oldItem) is >= 0 and var oldItemIndex)
                {
                    oldItemsData.Add(oldItemIndex, oldItem);
                }
            }

            var newItems = items.Except(Items).ToArray();

            if (newItems.Length == 0 && oldItemsData.Count == 0)
            {
                return;
            }

            var oldItems = oldItemsData.Values.ToArray();

            BeginSubscribe(oldItems);

            // Remove old
            foreach (var oldItemIndex in oldItemsData.Keys)
            {
                base.RemoveItem(oldItemIndex);
            }

            // Add new
            var newItemIndex = Count;
            foreach (var newItem in newItems)
            {
                base.InsertItem(newItemIndex, newItem);
                newItemIndex++;
            }

            EndSubscribe(oldItems, newItems);
        }

        public void MergeRange(IEnumerable<T> items)
        {
            var newItems = items.Except(Items).ToArray();

            if (newItems.Length == 0)
            {
                return;
            }

            var newItemIndex = Count;
            foreach (var newItem in newItems)
            {
                base.InsertItem(newItemIndex, newItem);
                newItemIndex++;
            }

            Subscribe(newItems);
        }

        public void AddRange(IEnumerable<T> items)
        {
            var newItems = items.ToArray();

            if (newItems.Length == 0)
            {
                return;
            }

            var newItemIndex = Count;
            foreach (var newItem in newItems)
            {
                base.InsertItem(newItemIndex, newItem);
                newItemIndex++;
            }

            Subscribe(newItems);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            var oldItemsData = new SortedList<int, T>(DescendingIndexComparer);
            foreach (var item in items)
            {
                if (IndexOf(item) is >= 0 and var index)
                {
                    oldItemsData.Add(index, item);
                }
            }

            if (oldItemsData.Count == 0)
            {
                return;
            }

            var oldItems = oldItemsData.Values.ToArray();

            BeginSubscribe(oldItems);

            foreach (var oldItemIndex in oldItemsData.Keys)
            {
                base.RemoveItem(oldItemIndex);
            }

            EndSubscribe(oldItems);
        }

        public void Merge(T item)
        {
            if (!Contains(item))
            {
                Add(item);
            }
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
            BeginSubscribe(oldItems);
            base.SetItem(index, item);
            EndSubscribe(oldItems, new[] { item, });
        }

        protected override void RemoveItem(int index)
        {
            var oldItems = new[] { Items[index], };
            BeginSubscribe(oldItems);
            base.RemoveItem(index);
            EndSubscribe(oldItems);
        }

        protected override void ClearItems()
        {
            var oldItems = Items.ToArray();
            BeginSubscribe(oldItems);
            base.ClearItems();
            EndSubscribe(oldItems);
        }

        private void OnCollectionItemChanged(CollectionItemChangedEventArgs<T> args)
        {
            CollectionItemChanged?.Invoke(this, args);
        }

        private void OnCollectionItemPropertyChanged(T item, CollectionItemPropertyChangedEventArgs args)
        {
            CollectionItemPropertyChanged?.Invoke(item, args);
        }

        private void Subscribe(T[] newItems)
        {
            OnCollectionItemChanged(new CollectionItemChangedEventArgs<T>(Array.Empty<T>(), newItems));
            SubscribeItemPropertyChanged(newItems);
        }

        private void BeginSubscribe(T[] oldItems)
        {
            UnsubscribeItemPropertyChanged(oldItems);
        }

        private void EndSubscribe(T[] oldItems)
        {
            OnCollectionItemChanged(new CollectionItemChangedEventArgs<T>(oldItems, Array.Empty<T>()));
        }

        private void EndSubscribe(T[] oldItems, T[] newItems)
        {
            OnCollectionItemChanged(new CollectionItemChangedEventArgs<T>(oldItems, newItems));
            SubscribeItemPropertyChanged(newItems);
        }

        private void SubscribeItemPropertyChanged(T[] items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void UnsubscribeItemPropertyChanged(T[] items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (sender is T item)
            {
                OnCollectionItemPropertyChanged(item, new CollectionItemPropertyChangedEventArgs(args.PropertyName));
            }
        }

        public event CollectionItemChangedEventHandler<T>? CollectionItemChanged;

        public event CollectionItemPropertyChangedEventHandler<T>? CollectionItemPropertyChanged;
    }
}
