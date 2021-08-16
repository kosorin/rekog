namespace Rekog.App.ObjectModel
{
    public delegate void CollectionItemChangedEventHandler<T>(IObservableObjectCollection<T> collection, CollectionItemChangedEventArgs<T> args)
        where T : ObservableObject;
}
