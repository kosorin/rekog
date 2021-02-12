using Rekog.App.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Rekog.App.ViewModel
{
    public class KeyViewModel : ViewModelBase<KeyModel>
    {
        private static readonly RotateTransform EmptyRotateTransform = new RotateTransform();
        private static readonly Geometry EmptyGeometry = new PathGeometry();

        public KeyViewModel(KeyModel model)
            : base(model)
        {
            UpdateLayout();
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        private Point _position;
        public Point Position
        {
            get => _position;
            set => Set(ref _position, value);
        }

        private Size _size;
        public Size Size
        {
            get => _size;
            set => Set(ref _size, value);
        }

        private RotateTransform _rotateTransform = EmptyRotateTransform;
        public RotateTransform RotateTransform
        {
            get => _rotateTransform;
            set => Set(ref _rotateTransform, value);
        }

        private Rect _bounds;
        public Rect Bounds
        {
            get => _bounds;
            private set => Set(ref _bounds, value);
        }

        private Geometry _shape = EmptyGeometry;
        public Geometry Shape
        {
            get => _shape;
            private set => Set(ref _shape, value);
        }

        private Geometry _steppedShape = EmptyGeometry;
        public Geometry SteppedShape
        {
            get => _steppedShape;
            set => Set(ref _steppedShape, value);
        }

        private Rect _actualBounds;
        public Rect ActualBounds
        {
            get => _actualBounds;
            private set => Set(ref _actualBounds, value);
        }

        private Geometry _actualShape = EmptyGeometry;
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
                UpdateLayout();
                break;
            }
        }

        private void UpdateLayout()
        {
            Position = new Point(Model.X, Model.Y);
            Size = new Size(Model.Width, Model.Height);
            RotateTransform = new RotateTransform(Model.RotationAngle, Model.RotationOriginX, Model.RotationOriginY);

            Shape = Model.GetShape().Clone();
            SteppedShape = Model.GetSteppedShape().Clone();
            Bounds = Shape.Bounds;

            var actualShape = Model.GetShape().Clone();
            var actualTransform = new TransformGroup();
            actualTransform.Children.Add(RotateTransform);
            actualTransform.Children.Add(new TranslateTransform(Position.X, Position.Y));
            actualShape.Transform = actualTransform;
            ActualShape = actualShape;
            ActualBounds = ActualShape.Bounds;
        }
    }
}
