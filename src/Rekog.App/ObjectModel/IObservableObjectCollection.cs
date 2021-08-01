using System.Collections;

namespace Rekog.App.ObjectModel
{
    public interface IObservableObjectCollection : ICollection
    {
        event CollectionItemChangedEventHandler? CollectionItemChanged;

        event CollectionItemPropertyChangedEventHandler? CollectionItemPropertyChanged;
    }
}
