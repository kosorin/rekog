namespace Rekog.App.ObjectModel
{
    public delegate void ListChangedEventHandler<TItem>(ObservableList<TItem> list, ListChangedEventArgs<TItem> args)
        where TItem : ObservableObject;
}
