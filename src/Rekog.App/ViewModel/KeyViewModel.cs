using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Koda.ColorTools;
using Koda.ColorTools.Wpf;
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
        private ObservableDictionary<LegendId, LegendViewModel> _legends = new ObservableDictionary<LegendId, LegendViewModel>();

        public KeyViewModel(KeyModel model)
            : base(model)
        {
            UpdateLayout();
            UpdateColor();
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

        public ObservableDictionary<LegendId, LegendViewModel> Legends
        {
            get => _legends;
            private set => Set(ref _legends, value);
        }

        protected override void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            base.OnModelPropertyChanged(sender, args);

            switch (args.PropertyName)
            {
                case nameof(KeyModel.X):
                case nameof(KeyModel.Y):
                case nameof(KeyModel.RotationAngle):
                case nameof(KeyModel.RotationOriginX):
                case nameof(KeyModel.RotationOriginY):
                case nameof(KeyModel.Width):
                case nameof(KeyModel.Height):
                case nameof(KeyModel.UseShape):
                case nameof(KeyModel.Shape):
                case nameof(KeyModel.IsStepped):
                case nameof(KeyModel.SteppedOffsetX):
                case nameof(KeyModel.SteppedOffsetY):
                case nameof(KeyModel.SteppedWidth):
                case nameof(KeyModel.SteppedHeight):
                case nameof(KeyModel.UseSteppedShape):
                case nameof(KeyModel.SteppedShape):
                case nameof(KeyModel.Roundness):
                case nameof(KeyModel.RoundConcaveCorner):
                case nameof(KeyModel.Margin):
                case nameof(KeyModel.InnerVerticalOffset):
                case nameof(KeyModel.Padding):
                    UpdateLayout();
                    break;
                case nameof(KeyModel.Color):
                    UpdateColor();
                    break;
            }
        }

        private void UpdateLayout()
        {
            var unitSize = App.UnitSize;

            Rotation = new RotateTransform(Model.RotationAngle, (Model.RotationOriginX - Model.X) * unitSize, (Model.RotationOriginY - Model.Y) * unitSize);

            Shape = GetShape(Model.GetGeometry());
            SteppedShape = Model.IsStepped ? GetShape(Model.GetSteppedGeometry()) : null;

            InnerShape = GetInnerShape(Model.GetSteppedGeometry());
            InnerShapeOffset = new Point(Model.InnerVerticalOffset * -Math.Sin(Math.PI * Model.RotationAngle / 180d), Model.InnerVerticalOffset * -Math.Cos(Math.PI * Model.RotationAngle / 180d));

            var actualTransform = new TransformGroup();
            actualTransform.Children.Add(new RotateTransform(Model.RotationAngle, Model.RotationOriginX - Model.X, Model.RotationOriginY - Model.Y));
            actualTransform.Children.Add(new TranslateTransform(Model.X, Model.Y));
            var actualShape = Model.GetGeometry().Clone();
            actualShape.Transform = actualTransform;
            ActualShape = actualShape;
            ActualBounds = ActualShape.Bounds;
        }

        private void UpdateColor()
        {
            Color = HexColor.TryParse(Model.Color, out var color) ? color.ToColor() : DefaultColor;
        }

        private PathGeometry GetShape(PathGeometry shape)
        {
            var unitSize = App.UnitSize;

            shape = shape.Clone();
            shape.Transform = new ScaleTransform(unitSize, unitSize);

            return shape
                .GetEnlargedPathGeometry(unitSize * -(Model.Roundness + Model.Margin), Model.RoundConcaveCorner)
                .GetEnlargedPathGeometry(unitSize * Model.Roundness, true);
        }

        private PathGeometry GetInnerShape(PathGeometry shape)
        {
            var unitSize = App.UnitSize;

            shape = shape.Clone();
            shape.Transform = new ScaleTransform(unitSize, unitSize);

            return shape
                .GetEnlargedPathGeometry(unitSize * -Model.Padding, false)
                .GetEnlargedPathGeometry(unitSize * -(Model.Roundness + Model.Margin), Model.RoundConcaveCorner)
                .GetEnlargedPathGeometry(unitSize * Model.Roundness, true);
        }
    }
}
