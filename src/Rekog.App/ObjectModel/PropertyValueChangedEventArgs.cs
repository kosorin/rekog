using System.ComponentModel;

namespace Rekog.App.ObjectModel
{
    public class PropertyValueChangedEventArgs : PropertyChangedEventArgs
    {
        public PropertyValueChangedEventArgs(string propertyName, object? newValue, object? oldValue)
            : base(propertyName)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }

        public object? NewValue { get; }

        public object? OldValue { get; }
    }
}
