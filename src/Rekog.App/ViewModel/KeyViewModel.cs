using Rekog.App.Model;
using Rekog.App.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Rekog.App.ViewModel
{
    public class KeyViewModel : ObservableObject
    {
        private KeyModel? _model;
        public KeyModel? Model
        {
            get => _model;
            set
            {
                if (Model is KeyModel oldValue)
                {
                    oldValue.PropertyChanged -= Model_PropertyChanged;
                }

                if (Set(ref _model, value, nameof(Model)))
                {
                    if (Model is KeyModel newValue)
                    {
                        newValue.PropertyChanged -= Model_PropertyChanged;
                        newValue.PropertyChanged += Model_PropertyChanged;
                    }

                    UpdateLayout();
                }
            }
        }

        public static double MinimumScale = 4d;
        public static double DefaultScale = 54d;

        private double _scale = DefaultScale;
        public double Scale
        {
            get => _scale;
            set
            {
                if (value < MinimumScale)
                {
                    value = MinimumScale;
                }

                if (Set(ref _scale, value, nameof(Scale)))
                {
                    UpdateLayout();
                }
            }
        }

        private Rect _bounds;
        public Rect Bounds
        {
            get => _bounds;
            private set => Set(ref _bounds, value, nameof(Bounds));
        }

        private Rect _rotatedBounds;
        public Rect RotatedBounds
        {
            get => _rotatedBounds;
            private set => Set(ref _rotatedBounds, value, nameof(RotatedBounds));
        }

        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            private set => Set(ref _rotationAngle, value, nameof(RotationAngle));
        }

        private Point _rotationOrigin;
        public Point RotationOrigin
        {
            get => _rotationOrigin;
            private set => Set(ref _rotationOrigin, value, nameof(RotationOrigin));
        }

        private PointCollection _shape = new();
        public PointCollection Shape
        {
            get => _shape;
            private set => Set(ref _shape, value, nameof(Shape));
        }

        private Rect _labelBounds;
        public Rect LabelBounds
        {
            get => _labelBounds;
            private set => Set(ref _labelBounds, value, nameof(LabelBounds));
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
            case nameof(KeyModel.X):
            case nameof(KeyModel.Y):
            case nameof(KeyModel.Width):
            case nameof(KeyModel.Height):
            case nameof(KeyModel.RotationAngle):
            case nameof(KeyModel.RotationOriginX):
            case nameof(KeyModel.RotationOriginY):
            case nameof(KeyModel.Shape):
                UpdateLayout();
                break;
            }
        }

        private void UpdateLayout()
        {
            if (Model == null)
            {
                return;
            }

            var shape = (Model.Shape ?? new ObservableCollection<Geometry.Point>(new Geometry.Point[]
            {
                new(0, 0),
                new(Model.Width, 0),
                new(Model.Width, Model.Height),
                new(0, Model.Height),
            })).Select(p => new Point(p.X, p.Y)).ToList();
            var rotatedShape = shape.Select(point =>
            {
                var dx = point.X - Model.RotationOriginX;
                var dy = point.Y - Model.RotationOriginY;
                var p = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                var angle = Model.RotationAngle * (Math.PI / 180);
                if (p != 0)
                {
                    angle += Math.Asin(dy / p);
                }

                var rx = p * Math.Cos(angle);
                var ry = p * Math.Sin(angle);

                return new Point(rx + Model.RotationOriginX, ry + Model.RotationOriginY);
            }).ToList();

            var shapeBounds = GetBounds(shape);
            Bounds = new Rect(
                new Point((Model.X + shapeBounds.X) * Scale, (Model.Y + shapeBounds.Y) * Scale),
                new Size(shapeBounds.Width * Scale, shapeBounds.Height * Scale));

            var rotatedShapeBounds = GetBounds(rotatedShape);
            RotatedBounds = new Rect(
                new Point((Model.X + rotatedShapeBounds.X) * Scale, (Model.Y + rotatedShapeBounds.Y) * Scale),
                new Size(rotatedShapeBounds.Width * Scale, rotatedShapeBounds.Height * Scale));
            RotationAngle = Model.RotationAngle;
            RotationOrigin = new Point((Model.RotationOriginX - shapeBounds.X) / shapeBounds.Width, (Model.RotationOriginY - shapeBounds.Y) / shapeBounds.Height);

            Shape = new PointCollection(shape.Select(p => new Point((p.X - shapeBounds.X) * Scale, (p.Y - shapeBounds.Y) * Scale)));
            LabelBounds = new Rect(
                new Point(-shapeBounds.X * Scale, -shapeBounds.Y * Scale),
                new Size(Model.Width * Scale, Model.Height * Scale));

            static Rect GetBounds(ICollection<Point> points)
            {
                var min = new Point(points.Min(p => p.X), points.Min(p => p.Y));
                var max = new Point(points.Max(p => p.X), points.Max(p => p.Y));
                var size = new Size(max.X - min.X, max.Y - min.Y);
                return new Rect(min, size);
            }
        }

        private static Color ParseColor(string color, Color fallbackColor)
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(color);
            }
            catch
            {
                return fallbackColor;
            }
        }
    }
}
