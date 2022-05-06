namespace Rekog.App.ObjectModel
{
    public class EntryPropertyChangedEventArgs<TKey, TValue>
        where TKey : notnull
        where TValue : ObservableObject
    {
        public EntryPropertyChangedEventArgs(TKey key, TValue value, string? propertyName)
        {
            Key = key;
            Value = value;
            PropertyName = propertyName;
        }

        public TKey Key { get; }

        public TValue Value { get; }

        public string? PropertyName { get; }
    }
}
