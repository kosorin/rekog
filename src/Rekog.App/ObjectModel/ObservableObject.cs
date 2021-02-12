using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rekog.App.ObjectModel
{
    public abstract class ObservableObject : IObservableObject
    {
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanging(string? propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T field, T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected bool SetCollection<T>(ref T field, T value,
            CollectionItemChangedEventHandler? onCollectionItemChanged = null,
            CollectionItemPropertyChangedEventHandler? onCollectionItemPropertyChanged = null,
            [CallerMemberName] string? propertyName = null)
            where T : class, IObservableObjectCollection?
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            var oldValue = field;

            OnPropertyChanging(propertyName);
            field = value;
            OnPropertyChanged(propertyName);

            var newValue = field;

            if (oldValue != null)
            {
                if (onCollectionItemChanged != null)
                {
                    oldValue.CollectionItemChanged -= onCollectionItemChanged;
                }
                if (onCollectionItemPropertyChanged != null)
                {
                    oldValue.CollectionItemPropertyChanged -= onCollectionItemPropertyChanged;
                }
            }
            if (newValue != null)
            {
                if (onCollectionItemChanged != null)
                {
                    newValue.CollectionItemChanged -= onCollectionItemChanged;
                    newValue.CollectionItemChanged += onCollectionItemChanged;
                }
                if (onCollectionItemPropertyChanged != null)
                {
                    newValue.CollectionItemPropertyChanged -= onCollectionItemPropertyChanged;
                    newValue.CollectionItemPropertyChanged += onCollectionItemPropertyChanged;
                }
            }

            return true;
        }
    }
}
