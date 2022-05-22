using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rekog.App.ObjectModel
{
    public abstract class ObservableObject : IObservableObject
    {
        public PropertyObserver? Observer { get; private set; }

        public void AttachObserver(PropertyObserver observer)
        {
            if (Observer != null)
            {
                throw new InvalidOperationException();
            }
            Observer = observer;
        }

        public void DetachObserver()
        {
            Observer = null;
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            var oldValue = field;

            OnPropertyChanging(propertyName);
            field = value;
            OnPropertyChanged(propertyName, value, oldValue);

            return true;
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(string propertyName, object? newValue, object? oldValue)
        {
            PropertyChanged?.Invoke(this, new PropertyValueChangedEventArgs(propertyName, newValue, oldValue));
        }

        public event PropertyChangingEventHandler? PropertyChanging;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
