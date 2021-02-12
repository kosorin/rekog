using System.ComponentModel;

namespace Rekog.App.ObjectModel
{
    public interface IObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
    }
}