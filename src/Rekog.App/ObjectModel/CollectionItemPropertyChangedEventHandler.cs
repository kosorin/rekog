namespace Rekog.App.ObjectModel
{
    public delegate void CollectionItemPropertyChangedEventHandler<in T>(T item, CollectionItemPropertyChangedEventArgs args)
        where T : ObservableObject;
}
