using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using PropertyTools.DataAnnotations;
using Rekog.App.Converters;
using Rekog.App.Model.Kle;
using Rekog.App.ObjectModel;

namespace Rekog.App.Model
{
    public class KeyModel : ModelBase
    {
        private string _group = string.Empty;
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

        [Category("Group")]
        [SortIndex(0)]
        public string Group
        {
            get => _group;
            set => Set(ref _group, value);
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Position")]
        [SortIndex(1)]
        public double X
        {
            get => _x;
            set => Set(ref _x, Math.Round(value, Precision));
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Position")]
        [SortIndex(2)]
        public double Y
        {
            get => _y;
            set => Set(ref _y, Math.Round(value, Precision));
        }

        [Spinnable(1, 10, -360, 360)]
        [Category("Rotation")]
        [DisplayName("Angle")]
        [SortIndex(11)]
        public double RotationAngle
        {
            get => _rotationAngle;
            set => Set(ref _rotationAngle, Math.Round(value, Precision));
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Rotation")]
        [DisplayName("Origin X")]
        [SortIndex(12)]
        public double RotationOriginX
        {
            get => _rotationOriginX;
            set => Set(ref _rotationOriginX, Math.Round(value, Precision));
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Rotation")]
        [DisplayName("Origin Y")]
        [SortIndex(13)]
        public double RotationOriginY
        {
            get => _rotationOriginY;
            set => Set(ref _rotationOriginY, Math.Round(value, Precision));
        }

        [Category("Shape")]
        [SortIndex(21)]
        public string? Shape
        {
            get => _shape;
            set => Set(ref _shape, value);
        }

        [Browsable(false)]
        public bool IsStepped => !string.IsNullOrWhiteSpace(SteppedShape);

        [Category("Shape")]
        [SortIndex(22)]
        public string? SteppedShape
        {
            get => _steppedShape;
            set => Set(ref _steppedShape, value);
        }

        [Spinnable(0.05, 0.25, 0.25, 10)]
        [Category("Simple shape")]
        [SortIndex(31)]
        public double Width
        {
            get => _width;
            set => Set(ref _width, Math.Round(value, Precision));
        }

        [Spinnable(0.05, 0.25, 0.25, 10)]
        [Category("Simple shape")]
        [SortIndex(32)]
        public double Height
        {
            get => _height;
            set => Set(ref _height, Math.Round(value, Precision));
        }

        [Category("Style")]
        [Converter(typeof(StringToColorConverter))]
        [SortIndex(41)]
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        [Spinnable(0.005, 0.025, 0, 0.2)]
        [Category("Style")]
        [SortIndex(42)]
        public double Roundness
        {
            get => _roundness;
            set => Set(ref _roundness, Math.Round(value, HighPrecision));
        }

        [Category("Style")]
        [SortIndex(43)]
        public bool RoundConcaveCorner
        {
            get => _roundConcaveCorner;
            set => Set(ref _roundConcaveCorner, value);
        }

        [Spinnable(0.005, 0.025, 0, 0.2)]
        [Category("Style")]
        [SortIndex(44)]
        public double Margin
        {
            get => _margin;
            set => Set(ref _margin, Math.Round(value, HighPrecision));
        }

        [Spinnable(0.005, 0.025, 0, 0.2)]
        [Category("Style")]
        [SortIndex(45)]
        public double Padding
        {
            get => _padding;
            set => Set(ref _padding, Math.Round(value, HighPrecision));
        }

        [Spinnable(0.005, 0.025, 0, 0.2)]
        [Category("Style")]
        [SortIndex(46)]
        public double InnerPadding
        {
            get => _innerPadding;
            set => Set(ref _innerPadding, Math.Round(value, HighPrecision));
        }

        [Spinnable(0.005, 0.025, -0.1, 0.1)]
        [Category("Style")]
        [SortIndex(47)]
        public double InnerVerticalOffset
        {
            get => _innerVerticalOffset;
            set => Set(ref _innerVerticalOffset, Math.Round(value, HighPrecision));
        }

        [Category("Misc")]
        [Browsable(false)]
        [SortIndex(91)]
        public bool IsHoming
        {
            get => _isHoming;
            set => Set(ref _isHoming, value);
        }

        [Category("Misc")]
        [Browsable(false)]
        [SortIndex(92)]
        public bool IsGhosted
        {
            get => _isGhosted;
            set => Set(ref _isGhosted, value);
        }

        [Category("Misc")]
        [Browsable(false)]
        [SortIndex(93)]
        public bool IsDecal
        {
            get => _isDecal;
            set => Set(ref _isDecal, value);
        }

        [Browsable(false)]
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

            var pathGeometry = geometry switch
            {
                PathGeometry pg => pg,
                Geometry g => g.GetFlattenedPathGeometry(),
            };

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

                    HorizontalAlignment = (System.Windows.HorizontalAlignment)(i % 3),
                    VerticalAlignment = (VerticalAlignment)(i / 3),

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
