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
    {
        private const string IndexerName = "Item";
        private static readonly IComparer<int> DescendingIndexComparer = Comparer<int>.Create((a, b) => b.CompareTo(a));
        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
        private static readonly PropertyChangedEventArgs IndexerPropertyChangedEventArgs = new PropertyChangedEventArgs(IndexerName + "[]");
        private static readonly NotifyCollectionChangedEventArgs ResetCollectionChangedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private static readonly IReadOnlyList<TItem> NoItems = Array.Empty<TItem>();

        private readonly IEqualityComparer<TItem> _equalityComparer;

        private readonly List<TItem> _list;

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
            return _list.Contains(item);
        }

        public int IndexOf(TItem item)
        {
            return _list.IndexOf(item);
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
            RemoveItem(index, out var item);

            OnListChanged(NoItems, new[] { item, });
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
            if (_list.IndexOf(item) is >= 0 and var index)
            {
                _list.RemoveAt(index);

                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnRemoveCollectionChanged(item, index);

                return true;
            }

            return false;
        }

        private void RemoveItem(int index, out TItem item)
        {
            if (index < 0 && index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            item = _list[index];

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
            ListChanged?.Invoke(this, new ListChangedEventArgs<TItem>(newItems, oldItems));
        }

        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public event ListChangedEventHandler<TItem>? ListChanged;
    }
}
