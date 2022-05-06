namespace Rekog.App.ObjectModel
{
    public delegate void EntryPropertyChangedEventHandler<TKey, TValue>(ObservableDictionary<TKey, TValue> dictionary, EntryPropertyChangedEventArgs<TKey, TValue> args)
        where TKey : notnull
        where TValue : ObservableObject;
}
