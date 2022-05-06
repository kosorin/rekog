using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Rekog.Common.Extensions;

namespace Rekog.App.ObjectModel
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
        where TKey : notnull
        where TValue : ObservableObject
    {
        private const string IndexerName = "Item";
        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
        private static readonly PropertyChangedEventArgs IndexerPropertyChangedEventArgs = new PropertyChangedEventArgs(IndexerName + "[]");
        private static readonly NotifyCollectionChangedEventArgs ResetCollectionChangedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private static readonly IReadOnlyDictionary<TKey, TValue> NoEntries = new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());

        private readonly IEqualityComparer<TValue> _equalityComparer;

        private readonly ObservableCollection<TKey> _keys;
        private readonly ObservableCollection<TValue> _values;
        private readonly Dictionary<TKey, int> _indices;

        private Dictionary<TKey, PropertyChangedEventHandler>? _observeEntriesHandlers;

        public ObservableDictionary() : this(EqualityComparer<TValue>.Default)
        {
        }

        public ObservableDictionary(IEqualityComparer<TValue> equalityComparer) : this(Array.Empty<KeyValuePair<TKey, TValue>>(), equalityComparer)
        {
        }

        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> entries) : this(entries, EqualityComparer<TValue>.Default)
        {
        }

        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> entries, IEqualityComparer<TValue> equalityComparer)
        {
            _equalityComparer = equalityComparer;

            var normalizedEntries = GetNormalizeEntries(entries);
            _keys = new ObservableCollection<TKey>(normalizedEntries.Select(x => x.Key));
            _values = new ObservableCollection<TValue>(normalizedEntries.Select(x => x.Value));
            _indices = normalizedEntries.Select((x, i) => (key: x.Key, index: i)).ToDictionary(x => x.key, x => x.index);

            Keys = new ReadOnlyObservableCollection<TKey>(_keys);
            Values = new ReadOnlyObservableCollection<TValue>(_values);
        }

        public int Count => _indices.Count;

        public ReadOnlyObservableCollection<TKey> Keys { get; }

        public ReadOnlyObservableCollection<TValue> Values { get; }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        [IndexerName(IndexerName)]
        public TValue this[TKey key]
        {
            get => Values[_indices[key]];
            set => Merge(key, value);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_indices.TryGetValue(key, out var index))
            {
                value = _values[index];
                return true;
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return _indices.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            AddEntry(key, value);

            var newEntries = new Dictionary<TKey, TValue>(1)
            {
                [key] = value,
            };

            OnDictionaryChanged(newEntries, NoEntries);
        }

        public bool Remove(TKey key)
        {
            if (RemoveEntry(key, out var value))
            {
                var oldEntries = new Dictionary<TKey, TValue>(1)
                {
                    [key] = value,
                };

                OnDictionaryChanged(NoEntries, oldEntries);

                return true;
            }

            return false;
        }

        public void Merge(TKey key, TValue value)
        {
            if (ReplaceEntry(key, value, out var oldValue))
            {
                if (!_equalityComparer.Equals(value, oldValue))
                {
                    var newEntries = new Dictionary<TKey, TValue>(1)
                    {
                        [key] = value,
                    };
                    var oldEntries = new Dictionary<TKey, TValue>(1)
                    {
                        [key] = oldValue,
                    };

                    OnDictionaryChanged(newEntries, oldEntries);
                }
            }
            else
            {
                Add(key, value);
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            var newEntries = new Dictionary<TKey, TValue>();

            foreach (var (key, newValue) in entries)
            {
                AddEntry(key, newValue);

                newEntries.Add(key, newValue);
            }

            if (newEntries.Count > 0)
            {
                OnDictionaryChanged(newEntries, NoEntries);
            }
        }

        public bool RemoveRange(IEnumerable<TKey> keys)
        {
            var oldEntries = new Dictionary<TKey, TValue>();

            foreach (var key in keys)
            {
                if (RemoveEntry(key, out var oldValue))
                {
                    oldEntries.Add(key, oldValue);
                }
            }

            if (oldEntries.Count > 0)
            {
                OnDictionaryChanged(NoEntries, oldEntries);
                return true;
            }

            return false;
        }

        public void MergeRange(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            var normalizedEntries = GetNormalizeEntries(entries);

            if (normalizedEntries.Count == 0)
            {
                return;
            }

            var newEntries = new Dictionary<TKey, TValue>();
            var oldEntries = new Dictionary<TKey, TValue>();

            foreach (var (key, newValue) in normalizedEntries)
            {
                if (ReplaceEntry(key, newValue, out var oldValue))
                {
                    if (!_equalityComparer.Equals(newValue, oldValue))
                    {
                        newEntries.Add(key, newValue);
                        oldEntries.Add(key, oldValue);
                    }
                }
                else
                {
                    AddEntry(key, newValue);

                    newEntries.Add(key, newValue);
                }
            }

            if (newEntries.Count > 0 || oldEntries.Count > 0)
            {
                OnDictionaryChanged(newEntries, oldEntries);
            }
        }

        public void ReplaceUsingMerge(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            var normalizedEntries = GetNormalizeEntries(entries);

            if (normalizedEntries.Count == 0 && Count == 0)
            {
                return;
            }

            var newEntries = new Dictionary<TKey, TValue>(normalizedEntries.Count);
            var oldEntries = new Dictionary<TKey, TValue>();

            foreach (var key in _keys.Except(normalizedEntries.Select(x => x.Key)).ToList())
            {
                if (RemoveEntry(key, out var oldValue))
                {
                    oldEntries.Add(key, oldValue);
                }
            }

            foreach (var (key, newValue) in normalizedEntries)
            {
                if (ReplaceEntry(key, newValue, out var oldValue))
                {
                    if (!_equalityComparer.Equals(newValue, oldValue))
                    {
                        newEntries.Add(key, newValue);
                        oldEntries.Add(key, oldValue);
                    }
                }
                else
                {
                    AddEntry(key, newValue);

                    newEntries.Add(key, newValue);
                }
            }

            if (newEntries.Count > 0 || oldEntries.Count > 0)
            {
                OnDictionaryChanged(newEntries, oldEntries);
            }
        }

        public void ReplaceUsingClear(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            var normalizedEntries = GetNormalizeEntries(entries);

            if (normalizedEntries.Count == 0 && Count == 0)
            {
                return;
            }

            var newEntries = new Dictionary<TKey, TValue>(normalizedEntries.Count);
            var oldEntries = this.ToDictionary(x => x.Key, x => x.Value);

            ClearEntries();

            foreach (var (key, newValue) in normalizedEntries)
            {
                AddEntry(key, newValue);

                if (oldEntries.TryGetValue(key, out var oldValue) && _equalityComparer.Equals(newValue, oldValue))
                {
                    oldEntries.Remove(key);
                }
                else
                {
                    newEntries.Add(key, newValue);
                }
            }

            if (newEntries.Count > 0 || oldEntries.Count > 0)
            {
                OnDictionaryChanged(newEntries, oldEntries);
            }
        }

        public void Clear()
        {
            if (Count == 0)
            {
                return;
            }

            var oldEntries = this.ToDictionary(x => x.Key, x => x.Value);

            ClearEntries();

            OnDictionaryChanged(NoEntries, oldEntries);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _keys.Zip(_values, (k, v) => new KeyValuePair<TKey, TValue>(k, v)).GetEnumerator();
        }

        private static List<KeyValuePair<TKey, TValue>> GetNormalizeEntries(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            // Make sure entries contains last value of each key
            return entries.Reverse().DistinctBy(x => x.Key).ToList();
        }

        private bool ReplaceEntry(TKey key, TValue newValue, [MaybeNullWhen(false)] out TValue oldValue)
        {
            if (_indices.TryGetValue(key, out var index))
            {
                oldValue = _values[index];

                _values[index] = newValue;

                OnIndexerPropertyChanged();
                OnReplaceCollectionChanged(key, newValue, oldValue, index);

                return true;
            }

            oldValue = default;
            return false;
        }

        private void AddEntry(TKey key, TValue value)
        {
            if (_indices.ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists.", nameof(key));
            }

            var index = Count;

            _keys.Add(key);
            _values.Add(value);
            _indices[key] = index;

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            OnAddCollectionChanged(key, value, index);
        }

        private bool RemoveEntry(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_indices.Remove(key, out var index))
            {
                value = _values[index];

                _keys.RemoveAt(index);
                _values.RemoveAt(index);
                foreach (var k in _keys)
                {
                    if (_indices[k] >= index)
                    {
                        _indices[k]--;
                    }
                }

                OnCountPropertyChanged();
                OnIndexerPropertyChanged();
                OnRemoveCollectionChanged(key, value, index);

                return true;
            }

            value = default;
            return false;
        }

        private void ClearEntries()
        {
            _keys.Clear();
            _values.Clear();
            _indices.Clear();

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

        private void OnAddCollectionChanged(TKey key, TValue value, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), index));
        }

        private void OnRemoveCollectionChanged(TKey key, TValue value, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value), index));
        }

        private void OnReplaceCollectionChanged(TKey key, TValue newValue, TValue oldValue, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, newValue), new KeyValuePair<TKey, TValue>(key, oldValue), index));
        }

        private void OnResetCollectionChanged()
        {
            OnCollectionChanged(ResetCollectionChangedEventArgs);
        }

        private void OnDictionaryChanged(IReadOnlyDictionary<TKey, TValue> newEntries, IReadOnlyDictionary<TKey, TValue> oldEntries)
        {
            if (_observeEntriesHandlers != null)
            {
                UnsubscribeEntryPropertyChanged(oldEntries);
                SubscribeEntryPropertyChanged(newEntries);
            }

            DictionaryChanged?.Invoke(this, new DictionaryChangedEventArgs<TKey, TValue>(newEntries, oldEntries));
        }

        private void SubscribeEntryPropertyChanged(IReadOnlyDictionary<TKey, TValue> entries)
        {
            if (_observeEntriesHandlers == null)
            {
                return;
            }

            foreach (var (key, value) in entries)
            {
                void Handler(object? _, PropertyChangedEventArgs args)
                {
                    EntryPropertyChangedCore?.Invoke(this, new EntryPropertyChangedEventArgs<TKey, TValue>(key, value, args.PropertyName));
                }

                if (_observeEntriesHandlers.TryAdd(key, Handler))
                {
                    value.PropertyChanged += Handler;
                }
            }
        }

        private void UnsubscribeEntryPropertyChanged(IReadOnlyDictionary<TKey, TValue> entries)
        {
            if (_observeEntriesHandlers == null)
            {
                return;
            }

            foreach (var (key, value) in entries)
            {
                if (_observeEntriesHandlers.Remove(key, out var handler))
                {
                    value.PropertyChanged -= handler;
                }
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _indices.TryGetValue(item.Key, out var index)
                && _equalityComparer.Equals(_values[index], item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return _indices.TryGetValue(item.Key, out var index)
                && _equalityComparer.Equals(_values[index], item.Value)
                && Remove(item.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array.Length < index + Count)
            {
                throw new ArgumentException(null, nameof(array));
            }

            for (var i = 0; i < Count; i++)
            {
                array[index + i] = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event DictionaryChangedEventHandler<TKey, TValue>? DictionaryChanged;

        public event EntryPropertyChangedEventHandler<TKey, TValue>? EntryPropertyChanged
        {
            add
            {
                if (_observeEntriesHandlers == null)
                {
                    _observeEntriesHandlers = new Dictionary<TKey, PropertyChangedEventHandler>();
                    SubscribeEntryPropertyChanged(this);
                }

                EntryPropertyChangedCore += value;
            }
            remove
            {
                EntryPropertyChangedCore -= value;

                if (_observeEntriesHandlers != null && EntryPropertyChangedCore == null)
                {
                    UnsubscribeEntryPropertyChanged(this);
                    Debug.Assert(_observeEntriesHandlers.Count == 0);
                    _observeEntriesHandlers = null;
                }
            }
        }

        private event EntryPropertyChangedEventHandler<TKey, TValue>? EntryPropertyChangedCore;

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
