namespace Rekog.App.ObjectModel.Forms
{
    public interface IFormProperty<T> : IObservableObject
    {
        bool IsSet { get; }

        T Value { get; set; }
    }
}
