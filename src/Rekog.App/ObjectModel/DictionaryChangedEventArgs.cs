using System.Collections.Generic;

namespace Rekog.App.ObjectModel
{
    public class DictionaryChangedEventArgs<TKey, TValue>
        where TKey : notnull
        where TValue : ObservableObject
    {
        public DictionaryChangedEventArgs(IReadOnlyDictionary<TKey, TValue> newEntries, IReadOnlyDictionary<TKey, TValue> oldEntries)
        {
            NewEntries = newEntries;
            OldEntries = oldEntries;
        }

        public IReadOnlyDictionary<TKey, TValue> NewEntries { get; }

        public IReadOnlyDictionary<TKey, TValue> OldEntries { get; }
    }
}
