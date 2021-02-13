using Rekog.App.Model.Kle;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Rekog.App.Model
{
    public class KeyModel : ModelBase
    {
        private double _x;
        public double X
        {
            get => _x;
            set => Set(ref _x, value);
        }

        private double _y;
        public double Y
        {
            get => _y;
            set => Set(ref _y, value);
        }

        private double _width = 1;
        public double Width
        {
            get => _width;
            set => Set(ref _width, value);
        }

        private double _height = 1;
        public double Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            set => Set(ref _rotationAngle, value);
        }

        private double _rotationOriginX;
        public double RotationOriginX
        {
            get => _rotationOriginX;
            set => Set(ref _rotationOriginX, value);
        }

        private double _rotationOriginY;
        public double RotationOriginY
        {
            get => _rotationOriginY;
            set => Set(ref _rotationOriginY, value);
        }

        private Geometry? _shape;
        public Geometry? Shape
        {
            get => _shape;
            set => Set(ref _shape, value);
        }

        private Geometry? _steppedShape;
        public Geometry? SteppedShape
        {
            get => _steppedShape;
            set => Set(ref _steppedShape, value);
        }

        private string _color = string.Empty;
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        private bool _isHoming;
        public bool IsHoming
        {
            get => _isHoming;
            set => Set(ref _isHoming, value);
        }

        private bool _isDecal;
        public bool IsDecal
        {
            get => _isDecal;
            set => Set(ref _isDecal, value);
        }

        private bool _isGhosted;
        public bool IsGhosted
        {
            get => _isGhosted;
            set => Set(ref _isGhosted, value);
        }

        private bool _isStepped;
        public bool IsStepped
        {
            get => _isStepped;
            set => Set(ref _isStepped, value);
        }

        public List<KeyLabelModel> Labels { get; set; } = new();

        public Geometry GetShape()
        {
            return Shape ?? GetDefaultShape();
        }

        public Geometry GetSteppedShape()
        {
            if (!IsStepped)
            {
                return GetShape();
            }
            return SteppedShape ?? GetDefaultShape();
        }

        private RectangleGeometry GetDefaultShape()
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

                Shape = kleKey.IsSimple ? null : GetShape(kleKey),
                SteppedShape = kleKey.IsSimple ? null : GetSteppedShape(kleKey),

                Color = kleKey.Color,
                IsHoming = kleKey.IsHoming,
                IsDecal = kleKey.IsDecal,
                IsGhosted = kleKey.IsGhosted,
                IsStepped = kleKey.IsStepped,
            };

            for (var i = 0; i < 9; i++)
            {
                if (kleKey.Labels[i] is not string text)
                {
                    continue;
                }

                var label = new KeyLabelModel
                {
                    Position = i,
                    Size = kleKey.TextSizes[i] ?? kleKey.DefaultTextSize,
                    Color = kleKey.TextColors[i] ?? kleKey.DefaultTextColor,
                    Text = text,
                };
                key.Labels.Add(label);
            }

            return key;

            static Geometry GetShape(KleKey kleKey)
            {
                var geometry1 = new RectangleGeometry(new Rect(0, 0, kleKey.Width, kleKey.Height));

                if (kleKey.IsSimple)
                {
                    return geometry1;
                }

                var geometry2 = new RectangleGeometry(new Rect(kleKey.X2, kleKey.Y2, kleKey.Width2, kleKey.Height2));

                return new CombinedGeometry(GeometryCombineMode.Union, geometry1, geometry2).GetFlattenedPathGeometry();
            }

            static Geometry GetSteppedShape(KleKey kleKey)
            {
                return new RectangleGeometry(new Rect(0, 0, kleKey.Width, kleKey.Height));
            }
        }
    }
}
