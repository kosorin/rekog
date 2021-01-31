using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Rekog.App.Views
{
    public partial class BoardView : UserControl
    {
        private Point _initialMousePosition;

        public BoardView()
        {
            InitializeComponent();
        }


        public static double GetScaleRatio(DependencyObject obj) => (double)obj.GetValue(ScaleRatioProperty);
        public static void SetScaleRatio(DependencyObject obj, double value) => obj.SetValue(ScaleRatioProperty, value);

        public static readonly DependencyProperty ScaleRatioProperty =
            DependencyProperty.RegisterAttached("ScaleRatio", typeof(double), typeof(BoardView), new PropertyMetadata(0d));


        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            private set => SetValue(IsDraggingProperty, value);
        }

        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register(nameof(IsDragging), typeof(bool), typeof(BoardView), new PropertyMetadata(false));

        public bool CanDrag
        {
            get => (bool)GetValue(CanDragProperty);
            private set => SetValue(CanDragProperty, value);
        }

        public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.Register(nameof(CanDrag), typeof(bool), typeof(BoardView), new PropertyMetadata(true));

        public double MinVisibleSize
        {
            get => (double)GetValue(MinVisibleSizeProperty);
            set => SetValue(MinVisibleSizeProperty, value);
        }

        public static readonly DependencyProperty MinVisibleSizeProperty =
            DependencyProperty.Register(nameof(MinVisibleSize), typeof(double), typeof(BoardView), new PropertyMetadata(128d));


        private void UserControl_PreviewMouseDown(object? sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Left && IsDragging)
            {
                args.Handled = true;
                IsDragging = false;

                CenterView();
            }
            else if (args.ChangedButton == MouseButton.Right)
            {
                if (CanDrag)
                {
                    args.Handled = true;
                    IsDragging = true;

                    _initialMousePosition = MatrixTransform.Inverse.Transform(args.GetPosition(PlateContainer));
                }
            }
            else
            {
                if (IsDragging)
                {
                    args.Handled = true;
                }
                else
                {
                    CanDrag = false;
                }
            }
        }

        private void UserControl_PreviewMouseUp(object? sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Right)
            {
                args.Handled = true;
                IsDragging = false;
            }
            else
            {
                if (IsDragging)
                {
                    args.Handled = true;
                }
                else
                {
                    CanDrag = true;
                }
            }
        }

        private void UserControl_PreviewMouseMove(object? sender, MouseEventArgs args)
        {
            if (args.RightButton == MouseButtonState.Pressed)
            {
                args.Handled = true;
                if (!IsDragging)
                {
                    return;
                }

                var mousePosition = MatrixTransform.Inverse.Transform(args.GetPosition(PlateContainer));
                var delta = Point.Subtract(mousePosition, _initialMousePosition);

                var matrix = MatrixTransform.Matrix;

                matrix.TranslatePrepend(delta.X, delta.Y);

                MatrixTransform.Matrix = matrix;

                KeepInView(args);
            }
            else
            {
                IsDragging = false;
                CanDrag = args.LeftButton == MouseButtonState.Released;
            }
        }

        private void UserControl_PreviewMouseWheel(object? sender, MouseWheelEventArgs args)
        {
            args.Handled = true;
            if (IsDragging)
            {
                return;
            }

            var matrix = MatrixTransform.Matrix;

            var scroll = 32 * Math.Sign(args.Delta) / matrix.M11;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                matrix.TranslatePrepend(scroll, 0);
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                matrix.TranslatePrepend(0, scroll);
            }
            else
            {
                var scale = args.Delta >= 0 ? 1.2 : (1.0 / 1.2);
                var position = args.GetPosition(Plate);

                matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
                if (matrix.M11 is < 0.2 or > 5.0)
                {
                    return;
                }
            }

            MatrixTransform.Matrix = matrix;

            KeepInView(args);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CenterView(reset: true);
        }

        private void PlateContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            KeepInView();
        }

        private void CenterView(bool reset = false)
        {
            if (reset)
            {
                MatrixTransform.Matrix = new Matrix();
            }

            (var plateContainerBounds, var plateBounds, var plateOffset) = GetPlateBounds(null);

            var matrix = MatrixTransform.Matrix;

            matrix.OffsetX = ((plateContainerBounds.Width - plateBounds.Width) / 2) - plateOffset.X;
            matrix.OffsetY = ((plateContainerBounds.Height - plateBounds.Height) / 2) - plateOffset.Y;

            MatrixTransform.Matrix = matrix;
        }

        private void KeepInView(MouseEventArgs? args = null)
        {
            (var plateContainerBounds, var plateBounds, var plateOffset) = GetPlateBounds(args);

            var minVisibleSize = new Size(
                Math.Min(plateBounds.Width, plateContainerBounds.Width),
                Math.Min(plateBounds.Height, plateContainerBounds.Height));

            var matrix = MatrixTransform.Matrix;
            var coerced = false;

            if (plateContainerBounds.Left + minVisibleSize.Width > plateBounds.Right)
            {
                matrix.OffsetX = minVisibleSize.Width - plateOffset.X - plateBounds.Width;
                coerced = true;
            }
            else if (plateContainerBounds.Right - minVisibleSize.Width < plateBounds.Left)
            {
                matrix.OffsetX = (plateContainerBounds.Right - minVisibleSize.Width) - plateOffset.X;
                coerced = true;
            }

            if (plateContainerBounds.Top + minVisibleSize.Height > plateBounds.Bottom)
            {
                matrix.OffsetY = minVisibleSize.Height - plateOffset.Y - plateBounds.Height;
                coerced = true;
            }
            else if (plateContainerBounds.Bottom - minVisibleSize.Height < plateBounds.Top)
            {
                matrix.OffsetY = (plateContainerBounds.Bottom - minVisibleSize.Height) - plateOffset.Y;
                coerced = true;
            }

            if (coerced)
            {
                MatrixTransform.Matrix = matrix;
            }
        }

        private (Rect plateContainerBounds, Rect plateBounds, Point plateOffset) GetPlateBounds(MouseEventArgs? args)
        {
            Point plateMousePosition;
            Point plateContainerMousePosition;

            if (args != null)
            {
                plateMousePosition = args.GetPosition(Plate);
                plateContainerMousePosition = args.GetPosition(PlateContainer);
            }
            else
            {
                plateMousePosition = new Point(0, 0);
                plateContainerMousePosition = Plate.TranslatePoint(plateMousePosition, PlateContainer);
            }

            var matrix = MatrixTransform.Matrix;
            var scale = matrix.M11;

            var plateContainerBounds = new Rect(
                new Point(0, 0),
                new Size(PlateContainer.ActualWidth, PlateContainer.ActualHeight));
            var plateBounds = new Rect(
                new Point(
                    plateContainerMousePosition.X - (plateMousePosition.X * scale),
                    plateContainerMousePosition.Y - (plateMousePosition.Y * scale)),
                new Size(Plate.ActualWidth * scale, Plate.ActualHeight * scale));
            var plateOffset = new Point(
                plateContainerMousePosition.X - ((plateMousePosition.X * scale) + matrix.OffsetX),
                plateContainerMousePosition.Y - ((plateMousePosition.Y * scale) + matrix.OffsetY));

            return (plateContainerBounds, plateBounds, plateOffset);
        }
    }
}
