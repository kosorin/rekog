using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    public class KeyControl : Control
    {
        static KeyControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyControl), new FrameworkPropertyMetadata(typeof(KeyControl)));
        }

        public KeyControl()
        {
        }

        public double UnitSize
        {
            get => (double)GetValue(UnitSizeProperty);
            set => SetValue(UnitSizeProperty, value);
        }

        public static readonly DependencyProperty UnitSizeProperty =
            DependencyProperty.Register(nameof(UnitSize), typeof(double), typeof(KeyControl), new PropertyMetadata(64d));

        public Point Position
        {
            get => (Point)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(Point), typeof(KeyControl), new PropertyMetadata(new Point(0, 0), OnPositionPropertyChanged));

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not KeyControl key)
            {
                return;
            }

            var unitSize = key.UnitSize;
            var position = key.Position;

            Canvas.SetLeft(key, position.X * unitSize);
            Canvas.SetTop(key, position.Y * unitSize);
        }

        public PointCollection Shape
        {
            get => (PointCollection)GetValue(ShapeProperty);
            set => SetValue(ShapeProperty, value);
        }

        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register(nameof(Shape), typeof(PointCollection), typeof(KeyControl), new PropertyMetadata(new PointCollection()));

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(KeyControl), new PropertyMetadata(Brushes.Silver));

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(KeyControl), new PropertyMetadata(Brushes.Black));

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(KeyControl), new PropertyMetadata(1d));

        public PointCollection SteppedShape
        {
            get => (PointCollection)GetValue(SteppedShapeProperty);
            set => SetValue(SteppedShapeProperty, value);
        }

        public static readonly DependencyProperty SteppedShapeProperty =
            DependencyProperty.Register(nameof(SteppedShape), typeof(PointCollection), typeof(KeyControl), new PropertyMetadata(new PointCollection()));

        public Brush SteppedFill
        {
            get => (Brush)GetValue(SteppedFillProperty);
            set => SetValue(SteppedFillProperty, value);
        }

        public static readonly DependencyProperty SteppedFillProperty =
            DependencyProperty.Register(nameof(SteppedFill), typeof(Brush), typeof(KeyControl), new PropertyMetadata(Brushes.DarkGray));

        public Brush SteppedStroke
        {
            get => (Brush)GetValue(SteppedStrokeProperty);
            set => SetValue(SteppedStrokeProperty, value);
        }

        public static readonly DependencyProperty SteppedStrokeProperty =
            DependencyProperty.Register(nameof(SteppedStroke), typeof(Brush), typeof(KeyControl), new PropertyMetadata(Brushes.DimGray));

        public double SteppedStrokeThickness
        {
            get => (double)GetValue(SteppedStrokeThicknessProperty);
            set => SetValue(SteppedStrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty SteppedStrokeThicknessProperty =
            DependencyProperty.Register(nameof(SteppedStrokeThickness), typeof(double), typeof(KeyControl), new PropertyMetadata(1d));
    }
}
