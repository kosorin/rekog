using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Rekog.App.ObjectModel
{
    public interface IObservableObjectCollection : ICollection, INotifyCollectionChanged, INotifyPropertyChanged
    {
        event CollectionItemChangedEventHandler? CollectionItemChanged;

        event CollectionItemPropertyChangedEventHandler? CollectionItemPropertyChanged;
    }
}
