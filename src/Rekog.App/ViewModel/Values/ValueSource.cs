using System;
using System.Runtime.CompilerServices;
using Rekog.App.ObjectModel;

namespace Rekog.App.ViewModel.Values
{
    public class ValueSource<T> : ObservableObject
    {
        private T _value;
        private bool _isSet;
        private int _version;

        public ValueSource(T value, [CallerMemberName] string? key = null)
        {
            _value = value;
            Key = key ?? throw new ArgumentNullException(nameof(key));
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

        public int Version
        {
            get => _version;
            set => Set(ref _version, value);
        }
    }
}
