using System.ComponentModel;
using Rekog.App.Extensions;
using Rekog.App.Forms;
using Rekog.App.Model;
using Rekog.App.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class KeyForm : ModelForm<KeyModel>
    {
        private double? _actualRotationOriginX;
        private double? _actualRotationOriginY;
        private bool _isUpdatingActualRotationOrigin;

        public KeyForm(UndoContext undoContext) : base(undoContext)
        {
        }

        public ModelFormProperty X => GetProperty(nameof(KeyModel.X));

        public ModelFormProperty Y => GetProperty(nameof(KeyModel.Y));

        public ModelFormProperty RotationAngle => GetProperty(nameof(KeyModel.RotationAngle));

        public ModelFormProperty RotationOriginX => GetProperty(nameof(KeyModel.RotationOriginX));

        public double? ActualRotationOriginX
        {
            get => _actualRotationOriginX;
            private set => Set(ref _actualRotationOriginX, value);
        }

        public double? ActualRotationOriginY
        {
            get => _actualRotationOriginY;
            private set => Set(ref _actualRotationOriginY, value);
        }

        public ModelFormProperty RotationOriginY => GetProperty(nameof(KeyModel.RotationOriginY));

        public ModelFormProperty Width => GetProperty(nameof(KeyModel.Width));

        public ModelFormProperty Height => GetProperty(nameof(KeyModel.Height));

        public ModelFormProperty UseShape => GetProperty(nameof(KeyModel.UseShape));

        public ModelFormProperty Shape => GetProperty(nameof(KeyModel.Shape));

        public ModelFormProperty IsStepped => GetProperty(nameof(KeyModel.IsStepped));

        public ModelFormProperty SteppedOffsetX => GetProperty(nameof(KeyModel.SteppedOffsetX));

        public ModelFormProperty SteppedOffsetY => GetProperty(nameof(KeyModel.SteppedOffsetY));

        public ModelFormProperty SteppedWidth => GetProperty(nameof(KeyModel.SteppedWidth));

        public ModelFormProperty SteppedHeight => GetProperty(nameof(KeyModel.SteppedHeight));

        public ModelFormProperty UseSteppedShape => GetProperty(nameof(KeyModel.UseSteppedShape));

        public ModelFormProperty SteppedShape => GetProperty(nameof(KeyModel.SteppedShape));

        public ModelFormProperty Color => GetProperty(nameof(KeyModel.Color));

        public ModelFormProperty Roundness => GetProperty(nameof(KeyModel.Roundness));

        public ModelFormProperty RoundConcaveCorner => GetProperty(nameof(KeyModel.RoundConcaveCorner));

        public ModelFormProperty Margin => GetProperty(nameof(KeyModel.Margin));

        public ModelFormProperty Padding => GetProperty(nameof(KeyModel.Padding));

        public ModelFormProperty InnerPadding => GetProperty(nameof(KeyModel.InnerPadding));

        public ModelFormProperty InnerVerticalOffset => GetProperty(nameof(KeyModel.InnerVerticalOffset));

        public ModelFormProperty IsHoming => GetProperty(nameof(KeyModel.IsHoming));

        public ModelFormProperty IsGhosted => GetProperty(nameof(KeyModel.IsGhosted));

        public ModelFormProperty IsDecal => GetProperty(nameof(KeyModel.IsDecal));

        protected override void OnModelsChanged()
        {
            _isUpdatingActualRotationOrigin = true;
            try
            {
                base.OnModelsChanged();

                UpdateActualRotationOriginX();
                UpdateActualRotationOriginY();
            }
            finally
            {
                _isUpdatingActualRotationOrigin = false;
            }
        }

        protected override void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            base.OnModelPropertyChanged(sender, args);

            if (_isUpdatingActualRotationOrigin)
            {
                return;
            }

            switch (args.PropertyName)
            {
                case nameof(KeyModel.X):
                case nameof(KeyModel.RotationOriginX):
                    UpdateActualRotationOriginX();
                    break;
                case nameof(KeyModel.Y):
                case nameof(KeyModel.RotationOriginY):
                    UpdateActualRotationOriginY();
                    break;
            }
        }

        protected override void OnPropertyValueChanged(ModelFormProperty property, object? value)
        {
            base.OnPropertyValueChanged(property, value);

            if (_isUpdatingActualRotationOrigin)
            {
                return;
            }

            switch (property.Name)
            {
                case nameof(KeyModel.X):
                case nameof(KeyModel.RotationOriginX):
                    UpdateActualRotationOriginX();
                    break;
                case nameof(KeyModel.Y):
                case nameof(KeyModel.RotationOriginY):
                    UpdateActualRotationOriginY();
                    break;
            }
        }

        private void UpdateActualRotationOriginX()
        {
            ActualRotationOriginX = Models.GetSameOrDefaultValue(x => x.RotationOriginX ?? x.X, (double?)null);
        }

        private void UpdateActualRotationOriginY()
        {
            ActualRotationOriginY = Models.GetSameOrDefaultValue(x => x.RotationOriginY ?? x.Y, (double?)null);
        }
    }
}
