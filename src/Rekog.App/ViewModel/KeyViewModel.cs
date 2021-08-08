﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Rekog.App.Extensions;
using Rekog.App.Model;
using Rekog.App.ObjectModel;

namespace Rekog.App.ViewModel
{
    public class KeyViewModel : ViewModelBase<KeyModel>
    {
        private static readonly RotateTransform EmptyRotateTransform = new RotateTransform();
        private static readonly PathGeometry EmptyGeometry = new PathGeometry();
        private static readonly Color DefaultColor = Colors.White;

        private bool _isSelected;
        private RotateTransform _rotation = EmptyRotateTransform;
        private PathGeometry _shape = EmptyGeometry;
        private PathGeometry _innerShape = EmptyGeometry;
        private PathGeometry? _steppedShape;
        private Point _innerShapeOffset;
        private Rect _actualBounds;
        private Geometry _actualShape = EmptyGeometry;
        private Color _color = DefaultColor;
        private ObservableObjectCollection<LegendViewModel> _legends = new ObservableObjectCollection<LegendViewModel>();

        public KeyViewModel(KeyModel model)
            : base(model)
        {
            UpdateAll();
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public RotateTransform Rotation
        {
            get => _rotation;
            private set => Set(ref _rotation, value);
        }

        public PathGeometry Shape
        {
            get => _shape;
            private set => Set(ref _shape, value);
        }

        public PathGeometry? SteppedShape
        {
            get => _steppedShape;
            private set => Set(ref _steppedShape, value);
        }

        public PathGeometry InnerShape
        {
            get => _innerShape;
            private set => Set(ref _innerShape, value);
        }

        public Point InnerShapeOffset
        {
            get => _innerShapeOffset;
            private set => Set(ref _innerShapeOffset, value);
        }

        public Rect ActualBounds
        {
            get => _actualBounds;
            private set => Set(ref _actualBounds, value);
        }

        public Geometry ActualShape
        {
            get => _actualShape;
            private set => Set(ref _actualShape, value);
        }

        public Color Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public ObservableObjectCollection<LegendViewModel> Legends
        {
            get => _legends;
            private set => SetCollection(ref _legends, value);
        }

        protected override void OnModelPropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
            base.OnModelPropertyChanging(sender, args);

            switch (args.PropertyName)
            {
                case nameof(KeyModel.Legends):
                    Model.Legends.CollectionItemChanged -= ModelLegends_CollectionItemChanged;
                    break;
            }
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
                case nameof(KeyModel.Roundness):
                case nameof(KeyModel.RoundConcaveCorner):
                case nameof(KeyModel.Margin):
                case nameof(KeyModel.SteppedShape):
                case nameof(KeyModel.InnerVerticalOffset):
                case nameof(KeyModel.Padding):
                    UpdateLayout();
                    break;
                case nameof(KeyModel.Color):
                    UpdateColor();
                    break;
                case nameof(KeyModel.Legends):
                    UpdateLegends();
                    break;
            }
        }

        private void UpdateAll()
        {
            UpdateLayout();
            UpdateColor();
            UpdateLegends();
        }

        private void UpdateLayout()
        {
            Rotation = new RotateTransform(Model.RotationAngle, Model.RotationOriginX - Model.X, Model.RotationOriginY - Model.Y);

            Shape = GetShape(Model.GetShapeGeometry());
            SteppedShape = Model.IsStepped ? GetShape(Model.GetSteppedShapeGeometry()) : null;

            InnerShape = GetInnerShape(Model.GetSteppedShapeGeometry());
            InnerShapeOffset = new Point(Model.InnerVerticalOffset * -Math.Sin(Math.PI * Model.RotationAngle / 180d), Model.InnerVerticalOffset * -Math.Cos(Math.PI * Model.RotationAngle / 180d));

            var actualTransform = new TransformGroup();
            actualTransform.Children.Add(Rotation);
            actualTransform.Children.Add(new TranslateTransform(Model.X, Model.Y));
            var actualShape = Model.GetShapeGeometry().Clone();
            actualShape.Transform = actualTransform;
            ActualShape = actualShape;
            ActualBounds = ActualShape.Bounds;
        }

        private void UpdateColor()
        {
            try
            {
                Color = Model.Color.ToColor();
            }
            catch
            {
                Color = DefaultColor;
            }
        }

        private void UpdateLegends()
        {
            Legends = new ObservableObjectCollection<LegendViewModel>(Model.Legends.Select(x => new LegendViewModel(x)));
            Model.Legends.CollectionItemChanged += ModelLegends_CollectionItemChanged;
        }

        private void ModelLegends_CollectionItemChanged(IObservableObjectCollection collection, CollectionItemChangedEventArgs args)
        {
            foreach (LegendModel oldLegendModel in args.OldItems)
            {
                for (var i = Legends.Count - 1; i >= 0; i--)
                {
                    if (Legends[i].Model == oldLegendModel)
                    {
                        Legends.RemoveAt(i);
                    }
                }
            }

            foreach (LegendModel newLegendModel in args.NewItems)
            {
                Legends.Add(new LegendViewModel(newLegendModel));
            }
        }

        private PathGeometry GetShape(PathGeometry shape)
        {
            var scale = App.UnitSize;

            shape = shape.Clone();
            shape.Transform = new ScaleTransform(scale, scale);

            return shape
                .GetEnlargedPathGeometry(scale * -(Model.Roundness + Model.Margin), Model.RoundConcaveCorner)
                .GetEnlargedPathGeometry(scale * Model.Roundness, true);
        }

        private PathGeometry GetInnerShape(PathGeometry shape)
        {
            var scale = App.UnitSize;

            shape = shape.Clone();
            shape.Transform = new ScaleTransform(scale, scale);

            return shape
                .GetEnlargedPathGeometry(scale * -Model.Padding, false)
                .GetEnlargedPathGeometry(scale * -(Model.Roundness + Model.Margin), Model.RoundConcaveCorner)
                .GetEnlargedPathGeometry(scale * Model.Roundness, true);
        }
    }
}
