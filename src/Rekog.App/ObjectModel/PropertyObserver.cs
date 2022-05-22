using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Rekog.App.ObjectModel
{
    public delegate void PropertyValueChangedHandler(PropertyObserver observer, Dictionary<PropertyValueChangeKey, PropertyValueChange> changes);

    public record PropertyValueChangeKey(ObservableObject Instance, string? PropertyName);

    public class PropertyValueChange
    {
        public PropertyValueChange(object? newValue, object? oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }

        public object? NewValue { get; set; }

        public object? OldValue { get; }
    }

    public class PropertyObserver
    {
        private readonly Dictionary<ObservableObject, int> _handlerCounts = new Dictionary<ObservableObject, int>();
        private readonly Dictionary<PropertyValueChangeKey, HashSet<PropertyValueChangedHandler>> _handlers = new Dictionary<PropertyValueChangeKey, HashSet<PropertyValueChangedHandler>>();
        private Dictionary<PropertyValueChangeKey, PropertyValueChange>? _changes;

        public IDisposable Coalesce()
        {
            return new CoalesceScope(this);
        }

        public void Subscribe(ObservableObject instance, string? propertyName, PropertyValueChangedHandler handler)
        {
            AddHandler(instance, propertyName, handler);
        }

        public void Unsubscribe(ObservableObject instance, string? propertyName, PropertyValueChangedHandler handler)
        {
            RemoveHandler(instance, propertyName, handler);
        }

        private void AddHandler(ObservableObject instance, string? propertyName, PropertyValueChangedHandler handler)
        {
            if (propertyName != null)
            {
                AddHandler(instance, null, handler);
            }

            var key = new PropertyValueChangeKey(instance, propertyName);

            if (!_handlers.TryGetValue(key, out var handlers))
            {
                _handlers.Add(key, handlers = new HashSet<PropertyValueChangedHandler>());
            }

            if (handlers.Add(handler))
            {
                if (_handlerCounts.ContainsKey(instance))
                {
                    _handlerCounts[instance]++;
                }
                else
                {
                    _handlerCounts.Add(instance, 1);
                    instance.PropertyChanged += OnInstancePropertyChanged;
                }
            }
        }

        private void RemoveHandler(ObservableObject instance, string? propertyName, PropertyValueChangedHandler handler)
        {
            if (propertyName != null)
            {
                RemoveHandler(instance, null, handler);
            }

            var key = new PropertyValueChangeKey(instance, propertyName);

            if (!_handlers.TryGetValue(key, out var handlers))
            {
                return;
            }

            if (handlers.Remove(handler))
            {
                if (_handlerCounts.ContainsKey(instance))
                {
                    if (--_handlerCounts[instance] == 0)
                    {
                        instance.PropertyChanged -= OnInstancePropertyChanged;
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private void OnInstancePropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (sender is not ObservableObject instance || args is not PropertyValueChangedEventArgs
                {
                    PropertyName: var propertyName,
                    NewValue: var newValue,
                    OldValue: var oldValue,
                })
            {
                return;
            }

            var key = new PropertyValueChangeKey(instance, propertyName);

            if (_changes == null)
            {
                if (_handlers.TryGetValue(key, out var handlers))
                {
                    var changes = new Dictionary<PropertyValueChangeKey, PropertyValueChange>
                    {
                        [new PropertyValueChangeKey(instance, propertyName)] = new PropertyValueChange(newValue, oldValue),
                    };

                    OnChanges(handlers, changes);
                }
            }
            else
            {
                if (_changes.TryGetValue(key, out var change))
                {
                    if (Equals(change.OldValue, newValue))
                    {
                        _changes.Remove(key);
                    }
                    else
                    {
                        change.NewValue = newValue;
                    }
                }
                else
                {
                    _changes.Add(key, new PropertyValueChange(newValue, oldValue));
                }
            }
        }

        private void OnChanges(ICollection<PropertyValueChangedHandler> handlers, Dictionary<PropertyValueChangeKey, PropertyValueChange> changes)
        {
            foreach (var handler in handlers)
            {
                handler.Invoke(this, changes);
            }
        }

        private void BeginCoalesce()
        {
            if (_changes != null)
            {
                throw new InvalidOperationException();
            }

            _changes = new Dictionary<PropertyValueChangeKey, PropertyValueChange>();
        }

        private void EndCoalesce()
        {
            if (_changes == null)
            {
                throw new InvalidOperationException();
            }

            var handlers = _handlers
                .Join(_changes.Keys, x => x.Key, x => x, (x, _) => x.Value)
                .SelectMany(x => x)
                .Distinct()
                .ToList();
            var changes = _changes;

            _changes = null;

            OnChanges(handlers, changes);
        }

        private class CoalesceScope : IDisposable
        {
            private readonly PropertyObserver _observer;

            public CoalesceScope(PropertyObserver observer)
            {
                _observer = observer;
                _observer.BeginCoalesce();
            }

            public void Dispose()
            {
                _observer.EndCoalesce();
            }
        }
    }
}
