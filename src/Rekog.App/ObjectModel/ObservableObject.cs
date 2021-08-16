using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rekog.App.ObjectModel
{
    public abstract class ObservableObject : IObservableObject
    {
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

        protected bool SetCollection<TCollection, TItem>(ref TCollection field, TCollection value,
            CollectionItemChangedEventHandler<TItem>? onCollectionItemChanged = null,
            CollectionItemPropertyChangedEventHandler<TItem>? onCollectionItemPropertyChanged = null,
            [CallerMemberName] string? propertyName = null)
            where TCollection : IObservableObjectCollection<TItem>
            where TItem : ObservableObject
        {
            if (EqualityComparer<TCollection>.Default.Equals(field, value))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            var oldValue = field;
            field = value;
            var newValue = field;
            OnPropertyChanged(propertyName);

            // Unregister events
            if (onCollectionItemChanged != null)
            {
                oldValue.CollectionItemChanged -= onCollectionItemChanged;
            }
            if (onCollectionItemPropertyChanged != null)
            {
                oldValue.CollectionItemPropertyChanged -= onCollectionItemPropertyChanged;
            }

            // Register events
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

            return true;
        }

        public event PropertyChangingEventHandler? PropertyChanging;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
