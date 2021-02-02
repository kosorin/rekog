using Rekog.App.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Rekog.App.ViewModel
{
    public class KeyViewModel : ViewModelBase<KeyModel>
    {
        public KeyViewModel()
        {
        }

        public KeyViewModel(KeyModel? model)
            : base(model)
        {
        }

        private Rect _bounds;
        public Rect Bounds
        {
            get => _bounds;
            private set => Set(ref _bounds, value);
        }

        private Rect _rotatedBounds;
        public Rect RotatedBounds
        {
            get => _rotatedBounds;
            private set => Set(ref _rotatedBounds, value);
        }

        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            private set => Set(ref _rotationAngle, value);
        }

        private Point _rotationOrigin;
        public Point RotationOrigin
        {
            get => _rotationOrigin;
            private set => Set(ref _rotationOrigin, value);
        }

        private PointCollection _shape = new();
        public PointCollection Shape
        {
            get => _shape;
            private set => Set(ref _shape, value);
        }

        private PointCollection _rotatedShape = new();
        public PointCollection RotatedShape
        {
            get => _rotatedShape;
            private set => Set(ref _rotatedShape, value);
        }

        private Rect _labelBounds;
        public Rect LabelBounds
        {
            get => _labelBounds;
            private set => Set(ref _labelBounds, value);
        }

        protected override void OnModelChanged(KeyModel? oldModel, KeyModel? newModel)
        {
            base.OnModelChanged(oldModel, newModel);

            UpdateLayout();
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

            var shape = (Model.Shape?.AsEnumerable() ?? new Geometry.Point[]
            {
                new(0, 0),
                new(Model.Width, 0),
                new(Model.Width, Model.Height),
                new(0, Model.Height),
            }).Select(p => new Point(p.X, p.Y)).ToList();
            var rotatedShape = shape.Select(point =>
            {
                var dx = point.X - Model.RotationOriginX;
                var dy = point.Y - Model.RotationOriginY;
                var p = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                var angle = Model.RotationAngle * (Math.PI / 180);
                if (p != 0)
                {
                    angle += Math.Atan2(dy, dx);
                }

                var ry = p * Math.Sin(angle);
                var rx = p * Math.Cos(angle);

                return new Point(rx + Model.RotationOriginX, ry + Model.RotationOriginY);
            }).ToList();

            var shapeBounds = GetBounds(shape);
            Bounds = new Rect(
                new Point(Model.X + shapeBounds.X, Model.Y + shapeBounds.Y),
                new Size(shapeBounds.Width, shapeBounds.Height));

            var rotatedShapeBounds = GetBounds(rotatedShape);
            RotatedBounds = new Rect(
                new Point(Model.X + rotatedShapeBounds.X, Model.Y + rotatedShapeBounds.Y),
                new Size(rotatedShapeBounds.Width, rotatedShapeBounds.Height));
            RotationAngle = Model.RotationAngle;
            RotationOrigin = new Point((Model.RotationOriginX - shapeBounds.X) / shapeBounds.Width, (Model.RotationOriginY - shapeBounds.Y) / shapeBounds.Height);

            Shape = new PointCollection(shape.Select(p => new Point(p.X - shapeBounds.X, p.Y - shapeBounds.Y)));
            RotatedShape = new PointCollection(rotatedShape.Select(p => new Point(p.X + Model.X, p.Y + Model.Y)));

            LabelBounds = new Rect(
                new Point(-shapeBounds.X, -shapeBounds.Y),
                new Size(Model.Width, Model.Height));

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
