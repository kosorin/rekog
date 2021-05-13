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
        private double _x;
        private double _y;
        private double _width = 1;
        private double _height = 1;
        private double _rotationAngle;
        private double _rotationOriginX;
        private double _rotationOriginY;
        private string? _shape;
        private double _roundness = 0.1;
        private bool _roundConcaveCorner = true;
        private double _margin = 0.02;
        private string? _steppedShape;
        private double _steppedOffset = 0.055;
        private double _steppedMargin = 0.095;
        private double _steppedPadding = 0.05;
        private string _color = "#FCFCFC";
        private bool _isHoming;
        private bool _isDecal;
        private bool _isGhosted;

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

        [Spinnable(0.05, 0.25, 0.25, 10)]
        [Category("Size")]
        [SortIndex(11)]
        public double Width
        {
            get => _width;
            set => Set(ref _width, Math.Round(value, Precision));
        }

        [Spinnable(0.05, 0.25, 0.25, 10)]
        [Category("Size")]
        [SortIndex(12)]
        public double Height
        {
            get => _height;
            set => Set(ref _height, Math.Round(value, Precision));
        }

        [Spinnable(1, 10, -360, 360)]
        [Category("Rotation")]
        [DisplayName("Angle")]
        [SortIndex(21)]
        public double RotationAngle
        {
            get => _rotationAngle;
            set => Set(ref _rotationAngle, Math.Round(value, Precision));
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Rotation")]
        [DisplayName("Origin X")]
        [SortIndex(22)]
        public double RotationOriginX
        {
            get => _rotationOriginX;
            set => Set(ref _rotationOriginX, Math.Round(value, Precision));
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Rotation")]
        [DisplayName("Origin Y")]
        [SortIndex(23)]
        public double RotationOriginY
        {
            get => _rotationOriginY;
            set => Set(ref _rotationOriginY, Math.Round(value, Precision));
        }

        [Category("Shape")]
        [SortIndex(41)]
        public string? Shape
        {
            get => _shape;
            set => Set(ref _shape, value);
        }

        [Spinnable(0.005, 0.025, 0, 0.2)]
        [Category("Shape")]
        [SortIndex(42)]
        public double Roundness
        {
            get => _roundness;
            set => Set(ref _roundness, Math.Round(value, HighPrecision));
        }

        [Category("Shape")]
        [SortIndex(43)]
        public bool RoundConcaveCorner
        {
            get => _roundConcaveCorner;
            set => Set(ref _roundConcaveCorner, value);
        }

        [Spinnable(0.005, 0.025, 0, 0.2)]
        [Category("Shape")]
        [SortIndex(44)]
        public double Margin
        {
            get => _margin;
            set => Set(ref _margin, Math.Round(value, HighPrecision));
        }

        [Browsable(false)]
        public bool IsStepped => !string.IsNullOrWhiteSpace(SteppedShape);

        [Category("Stepped")]
        [SortIndex(51)]
        public string? SteppedShape
        {
            get => _steppedShape;
            set => Set(ref _steppedShape, value);
        }

        [Spinnable(0.005, 0.025, -0.1, 0.1)]
        [Category("Stepped")]
        [SortIndex(52)]
        public double SteppedOffset
        {
            get => _steppedOffset;
            set => Set(ref _steppedOffset, Math.Round(value, HighPrecision));
        }

        [Spinnable(0.005, 0.025, 0, 0.2)]
        [Category("Stepped")]
        [SortIndex(53)]
        public double SteppedMargin
        {
            get => _steppedMargin;
            set => Set(ref _steppedMargin, Math.Round(value, HighPrecision));
        }

        [Spinnable(0.005, 0.025, 0, 0.2)]
        [Category("Stepped")]
        [SortIndex(54)]
        public double SteppedPadding
        {
            get => _steppedPadding;
            set => Set(ref _steppedPadding, Math.Round(value, HighPrecision));
        }

        [Category("Appearance")]
        [Converter(typeof(StringToColorConverter))]
        [SortIndex(31)]
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        [Category("Misc")]
        [Browsable(false)]
        public bool IsHoming
        {
            get => _isHoming;
            set => Set(ref _isHoming, value);
        }

        [Category("Misc")]
        [Browsable(false)]
        public bool IsDecal
        {
            get => _isDecal;
            set => Set(ref _isDecal, value);
        }

        [Category("Misc")]
        [Browsable(false)]
        public bool IsGhosted
        {
            get => _isGhosted;
            set => Set(ref _isGhosted, value);
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
                Width = kleKey.Width,
                Height = kleKey.Height,

                RotationAngle = kleKey.RotationAngle,
                RotationOriginX = kleKey.RotationX,
                RotationOriginY = kleKey.RotationY,

                Shape = kleKey.IsSimple ? null : GetShapePathGeometry(kleKey).ToString(CultureInfo.InvariantCulture),
                SteppedShape = kleKey.IsSimple || !kleKey.IsStepped ? null : GetSteppedShapePathGeometry(kleKey).ToString(CultureInfo.InvariantCulture),

                Color = kleKey.Color,
                IsHoming = kleKey.IsHoming,
                IsDecal = kleKey.IsDecal,
                IsGhosted = kleKey.IsGhosted,
            };

            for (var i = 0; i < 9; i++)
            {
                var label = new KeyLabelModel
                {
                    HorizontalAlignment = (System.Windows.HorizontalAlignment)(i % 3),
                    VerticalAlignment = (VerticalAlignment)(i / 3),
                    Size = 1.4 * (6 + 3 * (kleKey.TextSizes[i] ?? kleKey.DefaultTextSize)),
                    Color = kleKey.TextColors[i] ?? kleKey.DefaultTextColor,
                    Value = kleKey.Labels[i] ?? string.Empty,
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
