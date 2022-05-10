using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Rekog.App.ObjectModel
{
    public class ObservableList<TItem> : IList<TItem>, IReadOnlyList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
        where TItem : ObservableObject
    {
        private const string IndexerName = "Item";
        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
        private static readonly PropertyChangedEventArgs IndexerPropertyChangedEventArgs = new PropertyChangedEventArgs(IndexerName + "[]");
        private static readonly NotifyCollectionChangedEventArgs ResetCollectionChangedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private static readonly IReadOnlyList<TItem> NoItems = Array.Empty<TItem>();

        private readonly IEqualityComparer<TItem> _equalityComparer;

        private readonly List<TItem> _list;

        private Dictionary<TItem, PropertyChangedEventHandler>? _observedItemsHandlers;

        public ObservableList() : this(EqualityComparer<TItem>.Default)
        {
        }

        public ObservableList(IEqualityComparer<TItem> equalityComparer) : this(Array.Empty<TItem>(), equalityComparer)
        {
        }

        public ObservableList(IEnumerable<TItem> items) : this(items, EqualityComparer<TItem>.Default)
        {
        }

        public ObservableList(IEnumerable<TItem> items, IEqualityComparer<TItem> equalityComparer)
        {
            _equalityComparer = equalityComparer;

            _list = items.ToList();
        }

        public int Count => _list.Count;

        bool ICollection<TItem>.IsReadOnly => false;

        [IndexerName(IndexerName)]
        public TItem this[int index]
        {
            get => _list[index];
            set => Replace(index, value);
        }

        public bool Contains(TItem item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(TItem item)
        {
            return _list.FindIndex(x => _equalityComparer.Equals(x, item));
        }

        public void Insert(int index, TItem item)
        {
            InsertItem(index, item);

            OnListChanged(new[] { item, }, NoItems);
        }

        public void Replace(int index, TItem item)
        {
            ReplaceItem(index, item, out var oldItem);

            OnListChanged(new[] { item, }, new[] { oldItem, });
        }

        public void Add(TItem item)
        {
            AddItem(item);

            OnListChanged(new[] { item, }, NoItems);
        }

        public bool Remove(TItem item)
        {
            if (RemoveItem(item))
            {
                OnListChanged(NoItems, new[] { item, });

                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            RemoveItemAt(index, out var item);

            OnListChanged(NoItems, new[] { item, });
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            var newItems = new List<TItem>();

            AddRangeCore(items, newItems);

            if (newItems.Count > 0)
            {
                OnListChanged(newItems, NoItems);
            }
        }

        public void RemoveRange(IEnumerable<TItem> items)
        {
            var oldItems = new List<TItem>();

            RemoveRangeCore(items, oldItems);

            if (oldItems.Count > 0)
            {
                OnListChanged(NoItems, oldItems);
            }
        }

        public void MergeOverwrite(IEnumerable<TItem> items)
        {
            var enumeratedItems = items.ToList();

            if (enumeratedItems.Count == 0 && Count == 0)
            {
                return;
            }

            var newItems = new List<TItem>();
            var oldItems = new List<TItem>();

            // Removed items that are not in the enumeratedItems list
            var removeSet = new HashSet<TItem>(enumeratedItems, _equalityComparer);
            for (var i = Count - 1; i >= 0; i--)
            {
                var item = _list[i];
                if (removeSet.Add(item))
                {
                    RemoveItemWithoutCheck(i, item);

                    oldItems.Add(item);
                }
            }

            // Add items that are not in the list yet
            var addSet = new HashSet<TItem>(_list, _equalityComparer);
            foreach (var item in enumeratedItems)
            {
                if (addSet.Add(item))
                {
                    AddItem(item);

                    newItems.Add(item);
                }
            }

            if (newItems.Count > 0 || oldItems.Count > 0)
            {
                OnListChanged(newItems, oldItems);
            }
        }

        public void ClearOverwrite(IEnumerable<TItem> items)
        {
            var newItems = new List<TItem>();
            var oldItems = _list.ToList();

            ClearItems();

            AddRangeCore(items, newItems);

            if (newItems.Count > 0 || oldItems.Count > 0)
            {
                OnListChanged(newItems, oldItems);
            }
        }

        public void Clear()
        {
            if (Count == 0)
            {
                return;
            }

            var oldItems = this.ToList();

            ClearItems();

            OnListChanged(NoItems, oldItems);
        }

        public void CopyTo(TItem[] array, int index)
        {
            _list.CopyTo(array, index);
        }

        public List<TItem>.Enumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private void AddRangeCore(IEnumerable<TItem> items, List<TItem> newItems)
        {
            foreach (var item in items)
            {
                AddItem(item);

                newItems.Add(item);
            }
        }

        private void RemoveRangeCore(IEnumerable<TItem> items, List<TItem> oldItems)
        {
            foreach (var item in items)
            {
                if (RemoveItem(item))
                {
                    oldItems.Add(item);
                }
            }
        }

        private void InsertItem(int index, TItem item)
        {
            if (index < 0 && index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _list.Insert(index, item);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnAddOrInsertCollectionChanged(item, index);
        }

        private void ReplaceItem(int index, TItem newItem, out TItem oldItem)
        {
            if (index < 0 && index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            oldItem = _list[index];

            _list[index] = newItem;

            OnIndexerPropertyChanged();
            OnReplaceCollectionChanged(newItem, oldItem, index);
        }

        private void AddItem(TItem item)
        {
            var index = Count;

            _list.Add(item);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnAddOrInsertCollectionChanged(item, index);
        }

        private bool RemoveItem(TItem item)
        {
            if (IndexOf(item) is >= 0 and var index)
            {
                RemoveItemWithoutCheck(index, item);

                return true;
            }

            return false;
        }

        private void RemoveItemAt(int index, out TItem item)
        {
            if (index < 0 && index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            item = _list[index];

            RemoveItemWithoutCheck(index, item);
        }

        private void RemoveItemWithoutCheck(int index, TItem item)
        {
            _list.RemoveAt(index);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnRemoveCollectionChanged(item, index);
        }

        private void ClearItems()
        {
            _list.Clear();

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnResetCollectionChanged();
        }

        private void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        private void OnCountPropertyChanged()
        {
            OnPropertyChanged(CountPropertyChangedEventArgs);
        }

        private void OnIndexerPropertyChanged()
        {
            OnPropertyChanged(IndexerPropertyChangedEventArgs);
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        private void OnReplaceCollectionChanged(TItem newItem, TItem oldItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        }

        private void OnAddOrInsertCollectionChanged(TItem item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        private void OnRemoveCollectionChanged(TItem item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        private void OnResetCollectionChanged()
        {
            OnCollectionChanged(ResetCollectionChangedEventArgs);
        }

        private void OnListChanged(IReadOnlyList<TItem> newItems, IReadOnlyList<TItem> oldItems)
        {
            if (_observedItemsHandlers != null)
            {
                UnsubscribeItemPropertyChanged(oldItems);
                SubscribeItemPropertyChanged(newItems);
            }

            ListChanged?.Invoke(this, new ListChangedEventArgs<TItem>(newItems, oldItems));
        }

        private void SubscribeItemPropertyChanged(IReadOnlyList<TItem> items)
        {
            if (_observedItemsHandlers == null)
            {
                return;
            }

            foreach (var item in items)
            {
                void Handler(object? _, PropertyChangedEventArgs args)
                {
                    ItemPropertyChangedCore?.Invoke(this, new ItemPropertyChangedEventArgs<TItem>(item, args.PropertyName ?? throw new ArgumentNullException(nameof(args.PropertyName))));
                }

                _observedItemsHandlers.Add(item, Handler);
                item.PropertyChanged += Handler;
            }
        }

        private void UnsubscribeItemPropertyChanged(IReadOnlyList<TItem> items)
        {
            if (_observedItemsHandlers == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (_observedItemsHandlers.Remove(item, out var handler))
                {
                    item.PropertyChanged -= handler;
                }
            }
        }

        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event ListChangedEventHandler<TItem>? ListChanged;

        public event ItemPropertyChangedEventHandler<TItem>? ItemPropertyChanged
        {
            add
            {
                if (_observedItemsHandlers == null)
                {
                    _observedItemsHandlers = new Dictionary<TItem, PropertyChangedEventHandler>();
                    SubscribeItemPropertyChanged(this);
                }

                ItemPropertyChangedCore += value;
            }
            remove
            {
                ItemPropertyChangedCore -= value;

                if (_observedItemsHandlers != null && ItemPropertyChangedCore == null)
                {
                    UnsubscribeItemPropertyChanged(this);
                    _observedItemsHandlers = null;
                }
            }
        }

        private event ItemPropertyChangedEventHandler<TItem>? ItemPropertyChangedCore;

        private event PropertyChangedEventHandler? PropertyChanged;

        private event NotifyCollectionChangedEventHandler? CollectionChanged;

        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }

        event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
        {
            add => CollectionChanged += value;
            remove => CollectionChanged -= value;
        }
    }
}
