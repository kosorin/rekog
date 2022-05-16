using System;
using System.Runtime.CompilerServices;
using Rekog.App.ObjectModel;

namespace Rekog.App.ViewModel
{
    public class ValueSource<T> : ObservableObject
    {
        private T _value;
        private bool _isSet;

        public ValueSource(T value, [CallerMemberName] string? key = null)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _value = value;
        }

        public string Key { get; }

        public T Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public bool IsSet
        {
            get => _isSet;
            set => Set(ref _isSet, value);
        }
    }
}
