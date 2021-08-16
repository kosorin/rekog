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
        private ObservableObjectCollection<LegendModel> _legends = new ObservableObjectCollection<LegendModel>();

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

        public ObservableObjectCollection<LegendModel> Legends
        {
            get => _legends;
            set => Set(ref _legends, value);
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

        public static KeyModel FromKle(KleKey kleKey)
        {
            var x = kleKey.X;
            var y = kleKey.Y;
            var width = kleKey.Width;
            var height = kleKey.Height;
            var steppedOffsetX = 0d;
            var steppedOffsetY = 0d;
            var steppedWidth = kleKey.Width;
            var steppedHeight = kleKey.Height;
            string? shape = null;

            if (!kleKey.IsSimple)
            {
                if (kleKey.IsSimpleInverted)
                {
                    x = kleKey.X + kleKey.X2;
                    y = kleKey.Y + kleKey.Y2;
                    width = kleKey.Width2;
                    height = kleKey.Height2;
                    steppedOffsetX = -kleKey.X2;
                    steppedOffsetY = -kleKey.Y2;
                    steppedWidth = kleKey.Width;
                    steppedHeight = kleKey.Height;
                }
                else
                {
                    var geometry1 = new RectangleGeometry(new Rect(0, 0, kleKey.Width, kleKey.Height));
                    var geometry2 = new RectangleGeometry(new Rect(kleKey.X2, kleKey.Y2, kleKey.Width2, kleKey.Height2));
                    shape = new CombinedGeometry(GeometryCombineMode.Union, geometry1, geometry2).GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture);

                    // Delete "fill rule"
                    if (shape[0] == 'F')
                    {
                        shape = shape[2..];
                    }
                }
            }

            var key = new KeyModel
            {
                X = x,
                Y = y,

                RotationAngle = kleKey.RotationAngle,
                RotationOriginX = kleKey.RotationX,
                RotationOriginY = kleKey.RotationY,

                Width = width,
                Height = height,
                UseShape = shape != null,
                Shape = shape,

                IsStepped = kleKey.IsStepped,
                SteppedOffsetX = steppedOffsetX,
                SteppedOffsetY = steppedOffsetY,
                SteppedWidth = steppedWidth,
                SteppedHeight = steppedHeight,
                UseSteppedShape = false,
                SteppedShape = null,

                Color = kleKey.Color,

                IsHoming = kleKey.IsHoming,
                IsGhosted = kleKey.IsGhosted,
                IsDecal = kleKey.IsDecal,
            };

            for (var i = 0; i < 9; i++)
            {
                var legend = new LegendModel
                {
                    Value = kleKey.Legends[i] ?? string.Empty,

                    Alignment = (LegendAlignment)i,

                    Size = GetSize(kleKey.TextSizes[i] ?? kleKey.DefaultTextSize),
                    Color = kleKey.TextColors[i] ?? kleKey.DefaultTextColor,
                };
                key.Legends.Add(legend);
            }

            for (var i = 9; i < 12; i++)
            {
                var legend = new LegendModel
                {
                    Value = kleKey.Legends[i] ?? string.Empty,

                    Alignment = LegendAlignment.BottomLeft + (i - 9),
                    Bottom = -0.22,

                    Size = GetSize(kleKey.LegendTextSize),
                    Color = kleKey.TextColors[i] ?? kleKey.DefaultTextColor,
                };
                key.Legends.Add(legend);
            }

            return key;

            static double GetSize(double kleTextSize)
            {
                // Just trial and error
                return 8 + 4 * kleTextSize;
            }
        }
    }
}
