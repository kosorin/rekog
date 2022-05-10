namespace Rekog.App.ObjectModel
{
    public delegate void ItemPropertyChangedEventHandler<TItem>(ObservableList<TItem> list, ItemPropertyChangedEventArgs<TItem> args)
        where TItem : ObservableObject;
}
