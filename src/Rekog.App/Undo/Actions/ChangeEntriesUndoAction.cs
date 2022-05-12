using System.Collections.Generic;
using System.Linq;
using Rekog.App.ObjectModel;

namespace Rekog.App.Undo.Actions
{
    public class ChangeEntriesUndoAction<TKey, TValue> : IUndoAction
        where TKey : notnull
        where TValue : ObservableObject
    {
        private readonly ObservableDictionary<TKey, TValue> _dictionary;
        private readonly IReadOnlyList<KeyValuePair<TKey, TValue>> _newEntries;
        private readonly IReadOnlyList<KeyValuePair<TKey, TValue>> _oldEntries;

        public ChangeEntriesUndoAction(ObservableDictionary<TKey, TValue> dictionary, DictionaryChangedEventArgs<TKey, TValue> args)
        {
            _dictionary = dictionary;
            _newEntries = args.NewEntries.ToList();
            _oldEntries = args.OldEntries.ToList();
        }

        public void Undo()
        {
            _dictionary.Merge(_oldEntries, _newEntries.Select(x => x.Key));
        }

        public void Redo()
        {
            _dictionary.Merge(_newEntries, _oldEntries.Select(x => x.Key));
        }
    }
}
