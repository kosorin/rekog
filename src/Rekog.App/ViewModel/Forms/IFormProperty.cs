using Rekog.App.ObjectModel;

namespace Rekog.App.ViewModel.Forms
{
    public interface IFormProperty<T> : IObservableObject
    {
        bool IsSet { get; }

        T Value { get; set; }
    }
}
