using System.Collections;
using System.ComponentModel;

namespace Rekog.App.ObjectModel
{
    public interface IObservableObjectCollection : ICollection
    {
        event CollectionItemChangedEventHandler? CollectionItemChanged;

        event CollectionItemPropertyChangedEventHandler? CollectionItemPropertyChanged;
    }
}