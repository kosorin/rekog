using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Rekog.App.Model;

namespace Rekog.App.ViewModel
{
    public class KeyViewModel : ViewModelBase<KeyModel>
    {
        private static readonly RotateTransform EmptyRotateTransform = new RotateTransform();
        private static readonly Geometry EmptyGeometry = new PathGeometry();

        private bool _isSelected;
        private Point _position;
        private Size _size;
        private RotateTransform _rotateTransform = EmptyRotateTransform;
        private Rect _bounds;
        private Geometry _shape = EmptyGeometry;
        private Geometry _steppedShape = EmptyGeometry;
        private Rect _actualBounds;
        private Geometry _actualShape = EmptyGeometry;

        public KeyViewModel(KeyModel model)
            : base(model)
        {
            UpdateLayout();
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public Point Position
        {
            get => _position;
            private set => Set(ref _position, value);
        }

        public Size Size
        {
            get => _size;
            private set => Set(ref _size, value);
        }

        public RotateTransform RotateTransform
        {
            get => _rotateTransform;
            private set => Set(ref _rotateTransform, value);
        }

        public Rect Bounds
        {
            get => _bounds;
            private set => Set(ref _bounds, value);
        }

        public Geometry Shape
        {
            get => _shape;
            private set => Set(ref _shape, value);
        }

        public Geometry SteppedShape
        {
            get => _steppedShape;
            private set => Set(ref _steppedShape, value);
        }

        public Rect ActualBounds
        {
            get => _actualBounds;
            private set => Set(ref _actualBounds, value);
        }

        public Geometry ActualShape
        {
            get => _actualShape;
            private set => Set(ref _actualShape, value);
        }

        protected override void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            base.OnModelPropertyChanged(sender, args);

            switch (args.PropertyName)
            {
                case nameof(KeyModel.X):
                case nameof(KeyModel.Y):
                case nameof(KeyModel.Width):
                case nameof(KeyModel.Height):
                case nameof(KeyModel.RotationAngle):
                case nameof(KeyModel.RotationOriginX):
                case nameof(KeyModel.RotationOriginY):
                case nameof(KeyModel.Shape):
                case nameof(KeyModel.SteppedShape):
                case nameof(KeyModel.IsStepped):
                    UpdateLayout();
                    break;
            }
        }

        private void UpdateLayout()
        {
            Position = new Point(Model.X, Model.Y);
            Size = new Size(Model.Width, Model.Height);
            RotateTransform = new RotateTransform(Model.RotationAngle, Model.RotationOriginX - Model.X, Model.RotationOriginY - Model.Y);

            Shape = Model.GetShapeGeometry();
            SteppedShape = Model.GetSteppedShapeGeometry();
            Bounds = Shape.Bounds;

            var actualShape = Model.GetShapeGeometry().Clone();
            var actualTransform = new TransformGroup();
            actualTransform.Children.Add(RotateTransform);
            actualTransform.Children.Add(new TranslateTransform(Position.X, Position.Y));
            actualShape.Transform = actualTransform;
            ActualShape = actualShape;
            ActualBounds = ActualShape.Bounds;
        }
    }
}
