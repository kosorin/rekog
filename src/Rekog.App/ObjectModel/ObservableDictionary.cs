using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

        private readonly IEqualityComparer<TValue> _valueEqualityComparer;

        private readonly ObservableCollection<TKey> _keys;
        private readonly ObservableCollection<TValue> _values;
        private readonly Dictionary<TKey, int> _indices;

        private Dictionary<TKey, PropertyChangedEventHandler>? _observedEntriesHandlers;

        public ObservableDictionary() : this(EqualityComparer<TValue>.Default)
        {
        }

        public ObservableDictionary(IEqualityComparer<TValue> valueEqualityComparer) : this(Array.Empty<KeyValuePair<TKey, TValue>>(), valueEqualityComparer)
        {
        }

        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> entries) : this(entries, EqualityComparer<TValue>.Default)
        {
        }

        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> entries, IEqualityComparer<TValue> valueEqualityComparer)
        {
            _valueEqualityComparer = valueEqualityComparer;

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
            set => AddOrReplace(key, value);
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

        public void AddOrReplace(TKey key, TValue value)
        {
            if (ReplaceEntry(key, value, out var oldValue))
            {
                if (!_valueEqualityComparer.Equals(value, oldValue))
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

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            var newEntries = new Dictionary<TKey, TValue>();

            AddRangeCore(entries, newEntries);

            if (newEntries.Count > 0)
            {
                OnDictionaryChanged(newEntries, NoEntries);
            }
        }

        public void AddOrReplaceRange(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            var normalizedEntries = GetNormalizeEntries(entries);

            if (normalizedEntries.Count == 0)
            {
                return;
            }

            var newEntries = new Dictionary<TKey, TValue>();
            var oldEntries = new Dictionary<TKey, TValue>();

            AddOrReplaceRangeCore(normalizedEntries, newEntries, oldEntries);

            if (newEntries.Count > 0 || oldEntries.Count > 0)
            {
                OnDictionaryChanged(newEntries, oldEntries);
            }
        }

        public void RemoveRange(IEnumerable<TKey> keys)
        {
            var oldEntries = new Dictionary<TKey, TValue>();

            RemoveRangeCore(keys, oldEntries);

            if (oldEntries.Count > 0)
            {
                OnDictionaryChanged(NoEntries, oldEntries);
            }
        }

        public void Merge(IEnumerable<KeyValuePair<TKey, TValue>> addEntries, IEnumerable<TKey> removeKeys)
        {
            var normalizedAddEntries = GetNormalizeEntries(addEntries);
            var actualRemoveKeys = removeKeys.Except(normalizedAddEntries.Select(x => x.Key)).ToList();

            if (normalizedAddEntries.Count == 0 && actualRemoveKeys.Count == 0)
            {
                return;
            }

            var newEntries = new Dictionary<TKey, TValue>();
            var oldEntries = new Dictionary<TKey, TValue>();

            RemoveRangeCore(actualRemoveKeys, oldEntries);
            AddOrReplaceRangeCore(normalizedAddEntries, newEntries, oldEntries);

            if (newEntries.Count > 0 || oldEntries.Count > 0)
            {
                OnDictionaryChanged(newEntries, oldEntries);
            }
        }

        /// <summary>
        /// Replaces whole dictionary with new entries.
        /// </summary>
        /// <remarks>
        /// Removes entries from the dictionary that are not in <see cref="entries"/> and add or replace new entries.
        /// Same as <see cref="ClearOverwrite"/> but produce different <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/> events. 
        /// </remarks>
        public void MergeOverwrite(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            var normalizedEntries = GetNormalizeEntries(entries);

            if (normalizedEntries.Count == 0 && Count == 0)
            {
                return;
            }

            var removeKeys = _keys.Except(normalizedEntries.Select(x => x.Key)).ToList();

            var newEntries = new Dictionary<TKey, TValue>(normalizedEntries.Count);
            var oldEntries = new Dictionary<TKey, TValue>();

            RemoveRangeCore(removeKeys, oldEntries);
            AddOrReplaceRangeCore(normalizedEntries, newEntries, oldEntries);

            if (newEntries.Count > 0 || oldEntries.Count > 0)
            {
                OnDictionaryChanged(newEntries, oldEntries);
            }
        }

        /// <summary>
        /// Replaces whole dictionary with new entries.
        /// </summary>
        /// <remarks>
        /// Removes all entries from the dictionary and then add new entries.
        /// Same as <see cref="MergeOverwrite"/> but produce different <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/> events. 
        /// </remarks>
        public void ClearOverwrite(IEnumerable<KeyValuePair<TKey, TValue>> entries)
        {
            var normalizedEntries = GetNormalizeEntries(entries);

            if (normalizedEntries.Count == 0 && Count == 0)
            {
                return;
            }

            var newEntries = new Dictionary<TKey, TValue>(normalizedEntries.Count);
            var oldEntries = ToDictionary();

            ClearEntries();

            foreach (var (key, newValue) in normalizedEntries)
            {
                AddEntry(key, newValue);

                if (oldEntries.TryGetValue(key, out var oldValue) && _valueEqualityComparer.Equals(newValue, oldValue))
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

            var oldEntries = ToDictionary();

            ClearEntries();

            OnDictionaryChanged(NoEntries, oldEntries);
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return new Dictionary<TKey, TValue>(this);
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

        private void AddRangeCore(IEnumerable<KeyValuePair<TKey, TValue>> entries, Dictionary<TKey, TValue> newEntries)
        {
            foreach (var (key, newValue) in entries)
            {
                AddEntry(key, newValue);

                newEntries.Add(key, newValue);
            }
        }

        private void AddOrReplaceRangeCore(IEnumerable<KeyValuePair<TKey, TValue>> normalizedEntries, Dictionary<TKey, TValue> newEntries, Dictionary<TKey, TValue> oldEntries)
        {
            foreach (var (key, newValue) in normalizedEntries)
            {
                if (ReplaceEntry(key, newValue, out var oldValue))
                {
                    if (!_valueEqualityComparer.Equals(newValue, oldValue))
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
        }

        private void RemoveRangeCore(IEnumerable<TKey> removeKeys, Dictionary<TKey, TValue> oldEntries)
        {
            foreach (var key in removeKeys)
            {
                if (RemoveEntry(key, out var oldValue))
                {
                    oldEntries.Add(key, oldValue);
                }
            }
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
            if (_observedEntriesHandlers != null)
            {
                UnsubscribeEntryPropertyChanged(oldEntries);
                SubscribeEntryPropertyChanged(newEntries);
            }

            DictionaryChanged?.Invoke(this, new DictionaryChangedEventArgs<TKey, TValue>(newEntries, oldEntries));
        }

        private void SubscribeEntryPropertyChanged(IReadOnlyDictionary<TKey, TValue> entries)
        {
            if (_observedEntriesHandlers == null)
            {
                return;
            }

            foreach (var (key, value) in entries)
            {
                void Handler(object? _, PropertyChangedEventArgs args)
                {
                    EntryPropertyChangedCore?.Invoke(this, new EntryPropertyChangedEventArgs<TKey, TValue>(key, value, args.PropertyName ?? throw new ArgumentNullException(nameof(args.PropertyName))));
                }

                _observedEntriesHandlers.Add(key, Handler);
                value.PropertyChanged += Handler;
            }
        }

        private void UnsubscribeEntryPropertyChanged(IReadOnlyDictionary<TKey, TValue> entries)
        {
            if (_observedEntriesHandlers == null)
            {
                return;
            }

            foreach (var (key, value) in entries)
            {
                if (_observedEntriesHandlers.Remove(key, out var handler))
                {
                    value.PropertyChanged -= handler;
                }
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _indices.TryGetValue(item.Key, out var index)
                && _valueEqualityComparer.Equals(_values[index], item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return _indices.TryGetValue(item.Key, out var index)
                && _valueEqualityComparer.Equals(_values[index], item.Value)
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
                if (_observedEntriesHandlers == null)
                {
                    _observedEntriesHandlers = new Dictionary<TKey, PropertyChangedEventHandler>();
                    SubscribeEntryPropertyChanged(this);
                }

                EntryPropertyChangedCore += value;
            }
            remove
            {
                EntryPropertyChangedCore -= value;

                if (_observedEntriesHandlers != null && EntryPropertyChangedCore == null)
                {
                    UnsubscribeEntryPropertyChanged(this);
                    _observedEntriesHandlers = null;
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
