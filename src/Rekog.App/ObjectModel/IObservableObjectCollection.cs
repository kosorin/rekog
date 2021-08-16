using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Rekog.App.ObjectModel
{
    public interface IObservableObjectCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : ObservableObject
    {
        event CollectionItemChangedEventHandler<T>? CollectionItemChanged;

        event CollectionItemPropertyChangedEventHandler<T>? CollectionItemPropertyChanged;
    }
}
