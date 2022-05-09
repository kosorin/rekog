using Rekog.App.ViewModel;

namespace Rekog.App.Forms
{
    public class ModelFormProperty : ViewModelBase
    {
        private readonly ModelFormPropertyValueChangedCallback _valueChangedCallback;

        private object? _value;

        public ModelFormProperty(string name, object? defaultValue, ModelFormPropertyValueChangedCallback valueChangedCallback)
        {
            Name = name;
            DefaultValue = defaultValue;

            _valueChangedCallback = valueChangedCallback;
        }

        public string Name { get; }

        public object? DefaultValue { get; }

        public object? Value
        {
            get => _value;
            set
            {
                if (Set(ref _value, value))
                {
                    _valueChangedCallback.Invoke(this, value);
                }
            }
        }
    }
}
