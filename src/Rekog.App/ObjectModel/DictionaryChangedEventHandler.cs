namespace Rekog.App.ObjectModel
{
    public delegate void DictionaryChangedEventHandler<TKey, TValue>(ObservableDictionary<TKey, TValue> dictionary, DictionaryChangedEventArgs<TKey, TValue> args)
        where TKey : notnull;
}
