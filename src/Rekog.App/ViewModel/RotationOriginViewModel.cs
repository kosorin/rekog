using System.ComponentModel;
using System.Windows;
using Rekog.App.ViewModel.Forms;

namespace Rekog.App.ViewModel
{
    public class RotationOriginViewModel : ViewModelBase
    {
        private readonly KeyForm _keyForm;

        private bool _isSet;
        private Point _value;

        public RotationOriginViewModel(KeyForm keyForm)
        {
            _keyForm = keyForm;
            _keyForm.PropertyChanged += OnKeyFormPropertyChanged;

            Update();
        }

        public bool IsSet
        {
            get => _isSet;
            set => Set(ref _isSet, value);
        }

        public Point Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        private void OnKeyFormPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyForm.ActualRotationOriginX):
                case nameof(KeyForm.ActualRotationOriginY):
                    Update();
                    break;
            }
        }

        private void Update()
        {
            if (_keyForm.ActualRotationOriginX is { } x && _keyForm.ActualRotationOriginY is { } y)
            {
                Value = new Point(x, y);
                IsSet = true;
            }
            else
            {
                IsSet = false;
            }
        }
    }
}
