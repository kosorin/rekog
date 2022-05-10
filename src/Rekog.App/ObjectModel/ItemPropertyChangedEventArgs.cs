namespace Rekog.App.ObjectModel
{
    public class ItemPropertyChangedEventArgs<TItem>
        where TItem : ObservableObject
    {
        public ItemPropertyChangedEventArgs(TItem item, string propertyName)
        {
            Item = item;
            PropertyName = propertyName;
        }

        public TItem Item { get; }

        public string PropertyName { get; }
    }
}
