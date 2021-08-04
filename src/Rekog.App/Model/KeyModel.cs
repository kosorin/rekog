using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Rekog.App.Model.Kle;
using Rekog.App.ObjectModel;

namespace Rekog.App.Model
{
    public class KeyModel : ModelBase
    {
        private double _x;
        private double _y;
        private double _rotationAngle;
        private double _rotationOriginX;
        private double _rotationOriginY;
        private string? _shape;
        private string? _steppedShape;
        private double _width = 1;
        private double _height = 1;
        private string _color = "#FCFCFC";
        private double _roundness = 0.1;
        private bool _roundConcaveCorner = true;
        private double _margin = 0.02;
        private double _padding = 0.095;
        private double _innerPadding = 0.05;
        private double _innerVerticalOffset = 0.055;
        private bool _isHoming;
        private bool _isGhosted;
        private bool _isDecal;

        public double X
        {
            get => _x;
            set => Set(ref _x, Math.Round(value, Precision));
        }

        public double Y
        {
            get => _y;
            set => Set(ref _y, Math.Round(value, Precision));
        }

        public double RotationAngle
        {
            get => _rotationAngle;
            set => Set(ref _rotationAngle, Math.Round(value, Precision));
        }

        public double RotationOriginX
        {
            get => _rotationOriginX;
            set => Set(ref _rotationOriginX, Math.Round(value, Precision));
        }

        public double RotationOriginY
        {
            get => _rotationOriginY;
            set => Set(ref _rotationOriginY, Math.Round(value, Precision));
        }

        public string? Shape
        {
            get => _shape;
            set => Set(ref _shape, value);
        }

        public bool IsStepped => !string.IsNullOrWhiteSpace(SteppedShape);

        public string? SteppedShape
        {
            get => _steppedShape;
            set => Set(ref _steppedShape, value);
        }

        public double Width
        {
            get => _width;
            set => Set(ref _width, Math.Round(value, Precision));
        }

        public double Height
        {
            get => _height;
            set => Set(ref _height, Math.Round(value, Precision));
        }

        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public double Roundness
        {
            get => _roundness;
            set => Set(ref _roundness, Math.Round(value, HighPrecision));
        }

        public bool RoundConcaveCorner
        {
            get => _roundConcaveCorner;
            set => Set(ref _roundConcaveCorner, value);
        }

        public double Margin
        {
            get => _margin;
            set => Set(ref _margin, Math.Round(value, HighPrecision));
        }

        public double Padding
        {
            get => _padding;
            set => Set(ref _padding, Math.Round(value, HighPrecision));
        }

        public double InnerPadding
        {
            get => _innerPadding;
            set => Set(ref _innerPadding, Math.Round(value, HighPrecision));
        }

        public double InnerVerticalOffset
        {
            get => _innerVerticalOffset;
            set => Set(ref _innerVerticalOffset, Math.Round(value, HighPrecision));
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

        public ObservableObjectCollection<KeyLabelModel> Labels { get; set; } = new ObservableObjectCollection<KeyLabelModel>();

        public PathGeometry GetShapeGeometry()
        {
            return GetShapeGeometryCore(Shape);
        }

        public PathGeometry GetSteppedShapeGeometry()
        {
            var outerShapeGeometry = GetShapeGeometry();

            if (!IsStepped)
            {
                return outerShapeGeometry;
            }

            return GetShapeGeometryCore(SteppedShape, outerShapeGeometry);
        }

        private PathGeometry GetShapeGeometryCore(string? shape, Geometry? outerShapeGeometry = null)
        {
            var geometry = string.IsNullOrWhiteSpace(shape) ? GetDefaultShapeGeometry() : Geometry.Parse(shape);

            if (outerShapeGeometry != null)
            {
                geometry = new CombinedGeometry(GeometryCombineMode.Intersect, geometry, outerShapeGeometry);
            }

            var pathGeometry = geometry as PathGeometry ?? geometry.GetFlattenedPathGeometry();

            while (pathGeometry.Figures.Count > 1)
            {
                pathGeometry.Figures.RemoveAt(1);
            }

            return pathGeometry;
        }

        private Geometry GetDefaultShapeGeometry()
        {
            return new RectangleGeometry(new Rect(0, 0, Width, Height));
        }

        public static KeyModel FromKle(KleKey kleKey)
        {
            var key = new KeyModel
            {
                X = kleKey.X,
                Y = kleKey.Y,

                RotationAngle = kleKey.RotationAngle,
                RotationOriginX = kleKey.RotationX,
                RotationOriginY = kleKey.RotationY,

                Shape = kleKey.IsSimple ? null : GetShapePathGeometry(kleKey).ToString(CultureInfo.InvariantCulture),
                SteppedShape = kleKey.IsSimple || !kleKey.IsStepped ? null : GetSteppedShapePathGeometry(kleKey).ToString(CultureInfo.InvariantCulture),

                Width = kleKey.Width,
                Height = kleKey.Height,

                Color = kleKey.Color,

                IsHoming = kleKey.IsHoming,
                IsGhosted = kleKey.IsGhosted,
                IsDecal = kleKey.IsDecal,
            };

            for (var i = 0; i < 9; i++)
            {
                var label = new KeyLabelModel
                {
                    Value = kleKey.Labels[i] ?? string.Empty,

                    Alignment = (KeyLabelAlignment)i,

                    Size = 1.4 * (6 + 3 * (kleKey.TextSizes[i] ?? kleKey.DefaultTextSize)),
                    Color = kleKey.TextColors[i] ?? kleKey.DefaultTextColor,
                };
                key.Labels.Add(label);
            }

            return key;

            static PathGeometry GetShapePathGeometry(KleKey kleKey)
            {
                var geometry1 = new RectangleGeometry(new Rect(0, 0, kleKey.Width, kleKey.Height));

                if (kleKey.IsSimple)
                {
                    return geometry1.GetFlattenedPathGeometry();
                }

                var geometry2 = new RectangleGeometry(new Rect(kleKey.X2, kleKey.Y2, kleKey.Width2, kleKey.Height2));

                return new CombinedGeometry(GeometryCombineMode.Union, geometry1, geometry2).GetFlattenedPathGeometry();
            }

            static PathGeometry GetSteppedShapePathGeometry(KleKey kleKey)
            {
                return new RectangleGeometry(new Rect(0, 0, kleKey.Width, kleKey.Height)).GetFlattenedPathGeometry();
            }
        }
    }
}
