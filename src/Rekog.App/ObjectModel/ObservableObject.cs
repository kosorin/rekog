using System.Collections.Generic;
using System.ComponentModel;

namespace Rekog.App.ObjectModel
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private Queue<string?>? _suspended;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Suspend()
        {
            _suspended ??= new Queue<string?>();
        }

        protected void Resume()
        {
            if (_suspended != null)
            {
                var suspended = _suspended;
                _suspended = null;

                while (suspended.TryDequeue(out var propertyName))
                {
                    OnPropertyChanged(propertyName);
                }
            }
        }

        protected virtual void OnPropertyChanged(string? propertyName)
        {
            if (_suspended != null)
            {
                _suspended.Enqueue(propertyName);
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool Set<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
