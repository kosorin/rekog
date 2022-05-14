using System.Windows;
using System.Windows.Media;

namespace Rekog.App.Model
{
    public record KeyId(int Value)
    {
        public static implicit operator KeyId(int value)
        {
            return new KeyId(value);
        }
    }

    public class KeyModel : ModelBase
    {
        private double _x;
        private double _y;
        private double _rotationAngle;
        private double _rotationOriginX;
        private double _rotationOriginY;
        private double _width = 1;
        private double _height = 1;
        private bool _useShape;
        private string? _shape;
        private bool _isStepped;
        private double _steppedOffsetX;
        private double _steppedOffsetY;
        private double _steppedWidth = 1;
        private double _steppedHeight = 1;
        private bool _useSteppedShape;
        private string? _steppedShape;
        private string _color = "#FCFCFC";
        private double _roundness = 0.1;
        private bool _roundConcaveCorner = true;
        private double _margin = 0.0175;
        private double _padding = 0.08;
        private double _innerPadding = 0.05;
        private double _innerVerticalOffset = 0.08;
        private bool _isHoming;
        private bool _isGhosted;
        private bool _isDecal;

        public KeyModel(KeyId id)
        {
            Id = id;
        }

        public KeyId Id { get; }

        public Point Position => new Point(X, Y);

        public double X
        {
            get => _x;
            set => Set(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => Set(ref _y, value);
        }

        public double RotationAngle
        {
            get => _rotationAngle;
            set => Set(ref _rotationAngle, value);
        }

        public Point RotationOrigin => new Point(RotationOriginX, RotationOriginY);

        public double RotationOriginX
        {
            get => _rotationOriginX;
            set => Set(ref _rotationOriginX, value);
        }

        public double RotationOriginY
        {
            get => _rotationOriginY;
            set => Set(ref _rotationOriginY, value);
        }

        public double Width
        {
            get => _width;
            set => Set(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        public bool UseShape
        {
            get => _useShape;
            set => Set(ref _useShape, value);
        }

        public string? Shape
        {
            get => _shape;
            set => Set(ref _shape, value);
        }

        public bool IsStepped
        {
            get => _isStepped;
            set => Set(ref _isStepped, value);
        }

        public double SteppedOffsetX
        {
            get => _steppedOffsetX;
            set => Set(ref _steppedOffsetX, value);
        }

        public double SteppedOffsetY
        {
            get => _steppedOffsetY;
            set => Set(ref _steppedOffsetY, value);
        }

        public double SteppedWidth
        {
            get => _steppedWidth;
            set => Set(ref _steppedWidth, value);
        }

        public double SteppedHeight
        {
            get => _steppedHeight;
            set => Set(ref _steppedHeight, value);
        }

        public bool UseSteppedShape
        {
            get => _useSteppedShape;
            set => Set(ref _useSteppedShape, value);
        }

        public string? SteppedShape
        {
            get => _steppedShape;
            set => Set(ref _steppedShape, value);
        }

        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public double Roundness
        {
            get => _roundness;
            set => Set(ref _roundness, value);
        }

        // TODO: Consider removing RoundConcaveCorner property
        public bool RoundConcaveCorner
        {
            get => _roundConcaveCorner;
            set => Set(ref _roundConcaveCorner, value);
        }

        public double Margin
        {
            get => _margin;
            set => Set(ref _margin, value);
        }

        public double Padding
        {
            get => _padding;
            set => Set(ref _padding, value);
        }

        public double InnerPadding
        {
            get => _innerPadding;
            set => Set(ref _innerPadding, value);
        }

        public double InnerVerticalOffset
        {
            get => _innerVerticalOffset;
            set => Set(ref _innerVerticalOffset, value);
        }

        public bool IsHoming
        {
            get => _isHoming;
            set => Set(ref _isHoming, value);
        }

        public bool IsGhosted
        {
            get => _isGhosted;
            set => Set(ref _isGhosted, value);
        }

        public bool IsDecal
        {
            get => _isDecal;
            set => Set(ref _isDecal, value);
        }

        public PathGeometry GetGeometry()
        {
            var geometry = CreateGeometry(UseShape, Shape, 0, 0, Width, Height);

            var pathGeometry = geometry as PathGeometry ?? geometry.GetFlattenedPathGeometry();
            FixPathGeometry(pathGeometry);

            return pathGeometry;
        }

        public PathGeometry GetSteppedGeometry()
        {
            var outerGeometry = GetGeometry();

            if (!IsStepped)
            {
                return outerGeometry;
            }

            var geometry = CreateGeometry(UseSteppedShape, SteppedShape, SteppedOffsetX, SteppedOffsetY, SteppedWidth, SteppedHeight);
            geometry = new CombinedGeometry(GeometryCombineMode.Intersect, geometry, outerGeometry);

            var pathGeometry = geometry as PathGeometry ?? geometry.GetFlattenedPathGeometry();
            FixPathGeometry(pathGeometry);

            return pathGeometry;
        }

        private static Geometry CreateGeometry(bool useShape, string? shape, double x, double y, double width, double height)
        {
            return useShape && !string.IsNullOrWhiteSpace(shape)
                ? Geometry.Parse(shape)
                : new RectangleGeometry(new Rect(x, y, width, height));
        }

        private static void FixPathGeometry(PathGeometry pathGeometry)
        {
            while (pathGeometry.Figures.Count > 1)
            {
                pathGeometry.Figures.RemoveAt(1);
            }
        }
    }
}
