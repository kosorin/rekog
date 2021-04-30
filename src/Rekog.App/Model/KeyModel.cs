using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using PropertyTools.DataAnnotations;
using Rekog.App.Model.Kle;

namespace Rekog.App.Model
{
    public class KeyModel : ModelBase
    {
        private const int DoublePrecision = 2;

        private double _x;
        private double _y;
        private double _width = 1;
        private double _height = 1;
        private double _rotationAngle;
        private double _rotationOriginX;
        private double _rotationOriginY;
        private string? _shape;
        private string? _steppedShape;
        private string _color = string.Empty;
        private bool _isHoming;
        private bool _isDecal;
        private bool _isGhosted;
        private bool _isStepped;

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Position")]
        public double X
        {
            get => _x;
            set => Set(ref _x, Math.Round(value, DoublePrecision));
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Position")]
        public double Y
        {
            get => _y;
            set => Set(ref _y, Math.Round(value, DoublePrecision));
        }

        [Spinnable(0.05, 0.25, 0.25, 10)]
        [Category("Size")]
        public double Width
        {
            get => _width;
            set => Set(ref _width, Math.Round(value, DoublePrecision));
        }

        [Spinnable(0.05, 0.25, 0.25, 10)]
        [Category("Size")]
        public double Height
        {
            get => _height;
            set => Set(ref _height, Math.Round(value, DoublePrecision));
        }

        [Spinnable(1, 10, -360, 360)]
        [Category("Rotation")]
        [DisplayName("Angle")]
        public double RotationAngle
        {
            get => _rotationAngle;
            set => Set(ref _rotationAngle, Math.Round(value, DoublePrecision));
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Rotation")]
        [DisplayName("Origin X")]
        public double RotationOriginX
        {
            get => _rotationOriginX;
            set => Set(ref _rotationOriginX, Math.Round(value, DoublePrecision));
        }

        [Spinnable(0.05, 0.25, -100, 100)]
        [Category("Rotation")]
        [DisplayName("Origin Y")]
        public double RotationOriginY
        {
            get => _rotationOriginY;
            set => Set(ref _rotationOriginY, Math.Round(value, DoublePrecision));
        }

        [Category("Appearance")]
        public string? Shape
        {
            get => _shape;
            set => Set(ref _shape, value);
        }

        [Category("Appearance")]
        public string? SteppedShape
        {
            get => _steppedShape;
            set => Set(ref _steppedShape, value);
        }

        [Category("Appearance")]
        public string Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        [Category("Misc")]
        public bool IsHoming
        {
            get => _isHoming;
            set => Set(ref _isHoming, value);
        }

        [Category("Misc")]
        public bool IsDecal
        {
            get => _isDecal;
            set => Set(ref _isDecal, value);
        }

        [Category("Misc")]
        public bool IsGhosted
        {
            get => _isGhosted;
            set => Set(ref _isGhosted, value);
        }

        [Category("Appearance")]
        public bool IsStepped
        {
            get => _isStepped;
            set => Set(ref _isStepped, value);
        }

        [Browsable(false)]
        public List<KeyLabelModel> Labels { get; set; } = new List<KeyLabelModel>();

        public Geometry GetShapeGeometry()
        {
            return string.IsNullOrWhiteSpace(Shape) ? GetDefaultShapeGeometry() : Geometry.Parse(Shape);
        }

        public Geometry GetSteppedShapeGeometry()
        {
            if (!IsStepped)
            {
                return GetShapeGeometry();
            }
            return string.IsNullOrWhiteSpace(SteppedShape) ? GetDefaultShapeGeometry() : Geometry.Parse(SteppedShape);
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
                SteppedShape = kleKey.IsSimple ? null : GetSteppedShapePathGeometry(kleKey).ToString(CultureInfo.InvariantCulture),

                Color = kleKey.Color,
                IsHoming = kleKey.IsHoming,
                IsDecal = kleKey.IsDecal,
                IsGhosted = kleKey.IsGhosted,
                IsStepped = kleKey.IsStepped,
            };

            for (var i = 0; i < 9; i++)
            {
                if (kleKey.Labels[i] is not { } text)
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
