using Rekog.App.Geometry;
using Rekog.App.Model.Kle;
using Rekog.App.ObjectModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rekog.App.Model
{
    public class KeyModel : ObservableObject
    {
        private double _x;
        public double X
        {
            get => _x;
            set => Set(ref _x, value, nameof(X));
        }

        private double _y;
        public double Y
        {
            get => _y;
            set => Set(ref _y, value, nameof(Y));
        }

        private double _width;
        public double Width
        {
            get => _width;
            set => Set(ref _width, value, nameof(Width));
        }

        private double _height;
        public double Height
        {
            get => _height;
            set => Set(ref _height, value, nameof(Height));
        }

        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            set => Set(ref _rotationAngle, value, nameof(RotationAngle));
        }

        private double _rotationOriginX;
        public double RotationOriginX
        {
            get => _rotationOriginX;
            set => Set(ref _rotationOriginX, value, nameof(RotationOriginX));
        }

        private double _rotationOriginY;
        public double RotationOriginY
        {
            get => _rotationOriginY;
            set => Set(ref _rotationOriginY, value, nameof(RotationOriginY));
        }

        private ObservableCollection<Point>? _shape;
        public ObservableCollection<Point>? Shape
        {
            get => _shape;
            set => Set(ref _shape, value, nameof(Shape));
        }

        private ObservableCollection<Point>? _steppedShape;
        public ObservableCollection<Point>? SteppedShape
        {
            get => _steppedShape;
            set => Set(ref _steppedShape, value, nameof(SteppedShape));
        }

        private string _color = string.Empty;
        public string Color
        {
            get => _color;
            set => Set(ref _color, value, nameof(Color));
        }

        private bool _isHoming;
        public bool IsHoming
        {
            get => _isHoming;
            set => Set(ref _isHoming, value, nameof(IsHoming));
        }

        private bool _isDecal;
        public bool IsDecal
        {
            get => _isDecal;
            set => Set(ref _isDecal, value, nameof(IsDecal));
        }

        private bool _isGhosted;
        public bool IsGhosted
        {
            get => _isGhosted;
            set => Set(ref _isGhosted, value, nameof(IsGhosted));
        }

        public List<KeyLabelModel> Labels { get; set; } = new();


        public static KeyModel FromKle(KleKey kleKey)
        {
            var key = new KeyModel
            {
                X = kleKey.X,
                Y = kleKey.Y,
                Width = kleKey.Width,
                Height = kleKey.Height,

                RotationAngle = kleKey.RotationAngle,
                RotationOriginX = kleKey.RotationX - kleKey.X,
                RotationOriginY = kleKey.RotationY - kleKey.Y,

                Shape = kleKey.IsSimple ? null : PolygonToShape(kleKey.X, kleKey.Y, GetPolygon(kleKey)),
                SteppedShape = kleKey.IsSimple || !kleKey.IsStepped ? null : PolygonToShape(kleKey.X, kleKey.Y, GetSteppedPolygon(kleKey)),

                Color = kleKey.Color,
                IsHoming = kleKey.IsHoming,
                IsDecal = kleKey.IsDecal,
                IsGhosted = kleKey.IsGhosted,
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

            static ObservableCollection<Point> PolygonToShape(double X, double Y, Polygon polygon)
            {
                return new ObservableCollection<Point>(polygon.Vertices.Select(p => new Point(p.X - X, p.Y - Y)));
            }

            static Polygon GetPolygon(KleKey kleKey)
            {
                var polygon1 = new Polygon(new Point[]
                {
                    new(kleKey.X, kleKey.Y),
                    new(kleKey.X + kleKey.Width, kleKey.Y),
                    new(kleKey.X + kleKey.Width, kleKey.Y + kleKey.Height),
                    new(kleKey.X, kleKey.Y + kleKey.Height),
                });

                if (kleKey.IsSimple)
                {
                    return polygon1;
                }

                var polygon2 = new Polygon(new Point[]
                {
                    new(kleKey.X + kleKey.X2, kleKey.Y + kleKey.Y2),
                    new(kleKey.X + kleKey.X2 + kleKey.Width2, kleKey.Y + kleKey.Y2),
                    new(kleKey.X + kleKey.X2 + kleKey.Width2, kleKey.Y + kleKey.Y2 + kleKey.Height2),
                    new(kleKey.X + kleKey.X2, kleKey.Y + kleKey.Y2 + kleKey.Height2),
                });

                return polygon1.Union(polygon2).First();
            }

            static Polygon GetSteppedPolygon(KleKey kleKey)
            {
                return new Polygon(new Point[]
                {
                    new(kleKey.X, kleKey.Y),
                    new(kleKey.X + kleKey.Width, kleKey.Y),
                    new(kleKey.X + kleKey.Width, kleKey.Y + kleKey.Height),
                    new(kleKey.X, kleKey.Y + kleKey.Height),
                });
            }
        }
    }
}
