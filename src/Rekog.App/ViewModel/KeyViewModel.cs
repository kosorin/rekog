using System;
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
        private Point _position;
        private Size _size;
        private RotateTransform _rotateTransform = EmptyRotateTransform;
        private PathGeometry _shape = EmptyGeometry;
        private PathGeometry _steppedShape = EmptyGeometry;
        private PathGeometry? _steppedOuterShape;
        private Point _steppedOffset;
        private Rect _actualBounds;
        private Geometry _actualShape = EmptyGeometry;
        private Color _color = DefaultColor;
        private ObservableObjectCollection<KeyLabelViewModel> _labels = new ObservableObjectCollection<KeyLabelViewModel>();

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

        public Point Position
        {
            get => _position;
            private set => Set(ref _position, value);
        }

        public Size Size
        {
            get => _size;
            private set => Set(ref _size, value);
        }

        public RotateTransform RotateTransform
        {
            get => _rotateTransform;
            private set => Set(ref _rotateTransform, value);
        }

        public PathGeometry Shape
        {
            get => _shape;
            private set => Set(ref _shape, value);
        }

        public PathGeometry SteppedShape
        {
            get => _steppedShape;
            private set => Set(ref _steppedShape, value);
        }

        public PathGeometry? SteppedOuterShape
        {
            get => _steppedOuterShape;
            private set => Set(ref _steppedOuterShape, value);
        }

        public Point SteppedOffset
        {
            get => _steppedOffset;
            private set => Set(ref _steppedOffset, value);
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

        public ObservableObjectCollection<KeyLabelViewModel> Labels
        {
            get => _labels;
            private set => SetCollection(ref _labels, value);
        }

        protected override void OnModelPropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
            base.OnModelPropertyChanging(sender, args);

            switch (args.PropertyName)
            {
                case nameof(KeyModel.Labels):
                    Model.Labels.CollectionItemChanged -= ModelLabels_CollectionItemChanged;
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
                case nameof(KeyModel.SteppedOffset):
                case nameof(KeyModel.SteppedMargin):
                    UpdateLayout();
                    break;
                case nameof(KeyModel.Color):
                    UpdateColor();
                    break;
                case nameof(KeyModel.Labels):
                    UpdateLabels();
                    break;
            }
        }

        private void UpdateAll()
        {
            UpdateLayout();
            UpdateColor();
            UpdateLabels();
        }

        private void UpdateLayout()
        {
            Position = new Point(Model.X, Model.Y);
            Size = new Size(Model.Width, Model.Height);
            RotateTransform = new RotateTransform(Model.RotationAngle, Model.RotationOriginX - Model.X, Model.RotationOriginY - Model.Y);

            Shape = GetShapeConverter(Model.GetShapeGeometry());
            SteppedShape = GetSteppedShapeConverter(Model.GetSteppedShapeGeometry());
            SteppedOuterShape = Model.IsStepped ? GetShapeConverter(Model.GetSteppedShapeGeometry()) : null;
            SteppedOffset = new Point(Model.SteppedOffset * -Math.Sin(Math.PI * Model.RotationAngle / 180d), Model.SteppedOffset * -Math.Cos(Math.PI * Model.RotationAngle / 180d));

            var actualShape = Model.GetShapeGeometry().Clone();
            var actualTransform = new TransformGroup();
            actualTransform.Children.Add(RotateTransform);
            actualTransform.Children.Add(new TranslateTransform(Position.X, Position.Y));
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

        private void UpdateLabels()
        {
            Labels = new ObservableObjectCollection<KeyLabelViewModel>(Model.Labels.Select(x => new KeyLabelViewModel(x)));
            Model.Labels.CollectionItemChanged += ModelLabels_CollectionItemChanged;
        }

        private void ModelLabels_CollectionItemChanged(IObservableObjectCollection collection, CollectionItemChangedEventArgs args)
        {
            foreach (KeyLabelModel oldLabelModel in args.OldItems)
            {
                for (var i = Labels.Count - 1; i >= 0; i--)
                {
                    if (Labels[i].Model == oldLabelModel)
                    {
                        Labels.RemoveAt(i);
                    }
                }
            }

            foreach (KeyLabelModel newLabelModel in args.NewItems)
            {
                Labels.Add(new KeyLabelViewModel(newLabelModel));
            }
        }

        private PathGeometry GetShapeConverter(PathGeometry shape)
        {
            var scale = App.UnitSize;

            shape = shape.Clone();
            shape.Transform = new ScaleTransform(scale, scale);

            return shape
                .GetEnlargedPathGeometry(scale * -(Model.Roundness + Model.Margin), Model.RoundConcaveCorner)
                .GetEnlargedPathGeometry(scale * Model.Roundness, true);
        }

        private PathGeometry GetSteppedShapeConverter(PathGeometry shape)
        {
            var scale = App.UnitSize;

            shape = shape.Clone();
            shape.Transform = new ScaleTransform(scale, scale);

            return shape
                .GetEnlargedPathGeometry(scale * -Model.SteppedMargin, false)
                .GetEnlargedPathGeometry(scale * -(Model.Roundness + Model.Margin), Model.RoundConcaveCorner)
                .GetEnlargedPathGeometry(scale * Model.Roundness, true);
        }
    }
}
