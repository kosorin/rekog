using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Rekog.App.ObjectModel
{
    public interface IObservableObjectCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : ObservableObject
    {
        void ReplaceUsingClear(IEnumerable<T> items);

        void ReplaceUsingMerge(IEnumerable<T> items);

        void MergeRange(IEnumerable<T> items);

        void AddRange(IEnumerable<T> items);

        void RemoveRange(IEnumerable<T> items);

        void Merge(T item);

        event CollectionItemChangedEventHandler<T>? CollectionItemChanged;

        event CollectionItemPropertyChangedEventHandler<T>? CollectionItemPropertyChanged;
    }
}
