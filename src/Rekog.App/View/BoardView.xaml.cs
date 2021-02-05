using Rekog.App.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Rekog.App.View
{
    public partial class BoardView : UserControl
    {
        private SelectingContext? _selectingContext;
        private DraggingContext? _draggingContext;

        public BoardView()
        {
            InitializeComponent();
        }

        private double UnitSize => (double)Application.Current.Resources["UnitSize"];

        public static Thickness GetPlateCanvasOffset(DependencyObject obj) => (Thickness)obj.GetValue(PlateCanvasOffsetProperty);
        public static void SetPlateCanvasOffset(DependencyObject obj, Thickness value) => obj.SetValue(PlateCanvasOffsetProperty, value);

        public static readonly DependencyProperty PlateCanvasOffsetProperty =
            DependencyProperty.RegisterAttached("PlateCanvasOffset", typeof(Thickness), typeof(BoardView), new PropertyMetadata(new Thickness()));

        public static Rect GetBounds(DependencyObject obj) => (Rect)obj.GetValue(BoundsProperty);
        public static void SetBounds(DependencyObject obj, Rect value) => obj.SetValue(BoundsProperty, value);

        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.RegisterAttached("Bounds", typeof(Rect), typeof(BoardView), new PropertyMetadata(new Rect()));

        public static bool GetIsPreviewSelected(DependencyObject obj) => (bool)obj.GetValue(IsPreviewSelectedProperty);
        public static void SetIsPreviewSelected(DependencyObject obj, bool value) => obj.SetValue(IsPreviewSelectedProperty, value);

        public static readonly DependencyProperty IsPreviewSelectedProperty =
            DependencyProperty.RegisterAttached("IsPreviewSelected", typeof(bool), typeof(BoardView), new PropertyMetadata(false));

        public static bool GetIsSelected(DependencyObject obj) => (bool)obj.GetValue(IsSelectedProperty);
        public static void SetIsSelected(DependencyObject obj, bool value) => obj.SetValue(IsSelectedProperty, value);

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(BoardView), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public BoardState State
        {
            get => (BoardState)GetValue(StateProperty);
            private set => SetValue(StateProperty, value);
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(BoardState), typeof(BoardView), new PropertyMetadata(BoardState.None, OnStateChanged));

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is not BoardView boardView || args.NewValue is not BoardState state)
            {
                return;
            }

            if (state == BoardState.None)
            {
                if (boardView._selectingContext != null)
                {
                    foreach (var keyContainer in boardView._selectingContext.Keys)
                    {
                        keyContainer.IsPreviewSelected = false;
                    }
                    boardView._selectingContext = null;
                }

                boardView._draggingContext = null;
            }
        }

        private static bool IsShift() => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        private static bool IsCtrl() => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        private List<KeyContainer> GetKeyContainers()
        {
            var list = new List<KeyContainer>();

            foreach (var item in PlateCanvasContainer.Items)
            {
                var container = PlateCanvasContainer.ItemContainerGenerator.ContainerFromItem(item);
                if (container == null)
                {
                    PlateCanvasContainer.UpdateLayout();
                    container = PlateCanvasContainer.ItemContainerGenerator.ContainerFromItem(item);
                }

                if (container?.FindChild<Canvas>("Root") is Canvas canvas)
                {
                    list.Add(new KeyContainer(canvas));
                }
            }

            return list;
        }

        private KeyContainer? GetKeyContainer(MouseEventArgs? args)
        {
            if ((args?.OriginalSource as DependencyObject)?.FindParent<Canvas>("Root") is Canvas canvas)
            {
                return new KeyContainer(canvas);
            }

            return null;
        }

        private void UserControl_PreviewMouseDown(object? sender, MouseButtonEventArgs args)
        {
            if (State != BoardState.None)
            {
                args.Handled = true;
                return;
            }

            if (args.ChangedButton == MouseButton.Left)
            {
                ClearSelectionBox();

                args.Handled = true;
                State = BoardState.ClickSelecting;

                _selectingContext = new SelectingContext
                {
                    InitialMousePosition = args.GetPosition(PlateContainer),
                    InitialCoords = GetBoardCoords(args),
                    InitialKey = GetKeyContainer(args),
                    Keys = GetKeyContainers(),
                };
            }
            else if (args.ChangedButton == MouseButton.Right)
            {
                args.Handled = true;
                State = BoardState.Dragging;

                _draggingContext = new DraggingContext
                {
                    InitialMousePosition = MatrixTransform.Inverse.Transform(args.GetPosition(PlateContainer)),
                };
            }
        }

        private void UserControl_PreviewMouseUp(object? sender, MouseButtonEventArgs args)
        {
            if (State == BoardState.None)
            {
                return;
            }

            args.Handled = true;

            if (args.ChangedButton == MouseButton.Left && (State == BoardState.ClickSelecting || State == BoardState.DragSelecting))
            {
                switch (State)
                {
                case BoardState.ClickSelecting: SelectSingle(args); break;
                case BoardState.DragSelecting: SelectAll(args); break;
                }

                ClearSelectionBox();
            }
            else if (args.ChangedButton == MouseButton.Right && State == BoardState.Dragging)
            {
                MoveView(args);
            }

            State = BoardState.None;
        }

        private void UserControl_MouseLeave(object? sender, MouseEventArgs args)
        {
            State = BoardState.None;
        }

        private void UserControl_PreviewMouseMove(object? sender, MouseEventArgs args)
        {
            if (GetBoardCoords(args) is Point coords)
            {
                Coords.Text = $"{coords.X:N3}  {coords.Y:N3}";
            }

            if (args.LeftButton == MouseButtonState.Pressed && (State == BoardState.ClickSelecting || State == BoardState.DragSelecting))
            {
                args.Handled = true;

                ShowSelection(args);
            }
            else if (args.RightButton == MouseButtonState.Pressed && State == BoardState.Dragging)
            {
                args.Handled = true;

                MoveView(args);
            }
        }

        private void UserControl_PreviewMouseWheel(object? sender, MouseWheelEventArgs args)
        {
            if (State != BoardState.None)
            {
                return;
            }

            args.Handled = true;

            if (IsShift())
            {
                MoveView(Orientation.Horizontal, args.Delta);
            }
            else if (IsCtrl())
            {
                MoveView(Orientation.Vertical, args.Delta);
            }
            else
            {
                ZoomView(args);
            }
        }

        private void PlateContainer_SizeChanged(object? sender, SizeChangedEventArgs args)
        {
            KeepInView();
        }

        private void Plate_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            KeepInView();
        }

        private void ClearSelectionBox()
        {
            SelectionBox.Width = 0;
            SelectionBox.Height = 0;
        }

        private void ShowSelection(MouseEventArgs args)
        {
            if (_selectingContext?.InitialMousePosition is not Point initialMousePosition)
            {
                return;
            }

            var mousePosition = args.GetPosition(PlateContainer);
            var selectionBox = new Rect(
                new Point(
                    Math.Min(mousePosition.X, initialMousePosition.X),
                    Math.Min(mousePosition.Y, initialMousePosition.Y)),
                new Size(
                    Math.Abs(mousePosition.X - initialMousePosition.X),
                    Math.Abs(mousePosition.Y - initialMousePosition.Y)));

            if (State == BoardState.DragSelecting || SystemParameters.MinimumHorizontalDragDistance <= selectionBox.Width || SystemParameters.MinimumVerticalDragDistance <= selectionBox.Height)
            {
                State = BoardState.DragSelecting;

                Canvas.SetLeft(SelectionBox, selectionBox.X);
                Canvas.SetTop(SelectionBox, selectionBox.Y);
                SelectionBox.Width = selectionBox.Width;
                SelectionBox.Height = selectionBox.Height;

                SelectPreview(args);
            }
            else
            {
                ClearSelectionBox();
            }
        }

        private void SelectSingle(MouseEventArgs args)
        {
            if (!IsCtrl())
            {
                UnselectAll();
            }

            if (_selectingContext == null)
            {
                return;
            }

            if (_selectingContext.InitialKey is not KeyContainer initialKey || GetKeyContainer(args) is not KeyContainer key || initialKey.Container != key.Container)
            {
                return;
            }

            key.IsSelected ^= true;
        }

        private void SelectAll(MouseEventArgs args)
        {
            if (!IsCtrl())
            {
                UnselectAll();
            }

            if (_selectingContext == null)
            {
                return;
            }

            if (_selectingContext.InitialCoords is not Point initialCoords || GetBoardCoords(args) is not Point coords)
            {
                return;
            }

            var selectionBox = new Rect(
                new Point(
                    Math.Min(coords.X, initialCoords.X),
                    Math.Min(coords.Y, initialCoords.Y)),
                new Size(
                    Math.Abs(coords.X - initialCoords.X),
                    Math.Abs(coords.Y - initialCoords.Y)));

            foreach (var keyContainer in _selectingContext.Keys.Where(x => selectionBox.Contains(x.IsBounds)))
            {
                keyContainer.IsSelected = true;
            }
        }

        private void SelectPreview(MouseEventArgs args)
        {
            if (_selectingContext == null)
            {
                return;
            }

            foreach (var keyContainer in _selectingContext.Keys)
            {
                keyContainer.IsPreviewSelected = false;
            }

            if (_selectingContext.InitialCoords is not Point initialCoords || GetBoardCoords(args) is not Point coords)
            {
                return;
            }

            var selectionBox = new Rect(
                new Point(
                    Math.Min(coords.X, initialCoords.X),
                    Math.Min(coords.Y, initialCoords.Y)),
                new Size(
                    Math.Abs(coords.X - initialCoords.X),
                    Math.Abs(coords.Y - initialCoords.Y)));

            foreach (var keyContainer in _selectingContext.Keys.Where(x => selectionBox.Contains(x.IsBounds)))
            {
                keyContainer.IsPreviewSelected = true;
            }
        }

        private void UnselectAll()
        {
            if (_selectingContext != null)
            {
                foreach (var keyContainer in _selectingContext.Keys)
                {
                    keyContainer.IsPreviewSelected = false;
                    keyContainer.IsSelected = false;
                }
            }
        }

        private void MoveView(MouseEventArgs args)
        {
            if (_draggingContext == null)
            {
                return;
            }

            var mousePosition = MatrixTransform.Inverse.Transform(args.GetPosition(PlateContainer));
            var delta = Point.Subtract(mousePosition, _draggingContext.InitialMousePosition);

            var matrix = MatrixTransform.Matrix;

            matrix.TranslatePrepend(delta.X, delta.Y);

            MatrixTransform.Matrix = matrix;

            KeepInView();
        }

        private void MoveView(Orientation orientation, int delta)
        {
            if (delta == 0)
            {
                return;
            }

            var matrix = MatrixTransform.Matrix;

            var scroll = UnitSize * 0.5 * Math.Sign(delta) / matrix.M11;
            switch (orientation)
            {
            case Orientation.Horizontal:
                matrix.TranslatePrepend(scroll, 0);
                break;
            case Orientation.Vertical:
                matrix.TranslatePrepend(0, scroll);
                break;
            }

            MatrixTransform.Matrix = matrix;

            KeepInView();
        }

        private void ZoomView(MouseWheelEventArgs args)
        {
            var matrix = MatrixTransform.Matrix;

            var factor = 1.2;
            var scale = args.Delta >= 0 ? factor : (1.0 / factor);
            var position = args.GetPosition(Plate);

            matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            if (matrix.M11 is < 0.2 or > 5.0)
            {
                return;
            }

            MatrixTransform.Matrix = matrix;

            KeepInView();
        }

        public void CenterView(bool reset = false)
        {
            if (reset)
            {
                MatrixTransform.Matrix = new Matrix();
            }

            (var plateContainerBounds, var plateBounds, var plateOffset) = GetPlateBounds();

            var matrix = MatrixTransform.Matrix;

            matrix.OffsetX = ((plateContainerBounds.Width - plateBounds.Width) / 2) - plateOffset.X;
            matrix.OffsetY = ((plateContainerBounds.Height - plateBounds.Height) / 2) - plateOffset.Y;

            MatrixTransform.Matrix = matrix;
        }

        public void KeepInView()
        {
            (var plateContainerBounds, var plateBounds, var plateOffset) = GetPlateBounds();

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
                matrix.OffsetX = plateContainerBounds.Right - minVisibleSize.Width - plateOffset.X;
                coerced = true;
            }

            if (plateContainerBounds.Top + minVisibleSize.Height > plateBounds.Bottom)
            {
                matrix.OffsetY = minVisibleSize.Height - plateOffset.Y - plateBounds.Height;
                coerced = true;
            }
            else if (plateContainerBounds.Bottom - minVisibleSize.Height < plateBounds.Top)
            {
                matrix.OffsetY = plateContainerBounds.Bottom - minVisibleSize.Height - plateOffset.Y;
                coerced = true;
            }

            if (coerced)
            {
                MatrixTransform.Matrix = matrix;
            }
        }

        private (Rect plateContainerBounds, Rect plateBounds, Point plateOffset) GetPlateBounds()
        {
            var plateMousePosition = new Point(0, 0);
            var plateContainerMousePosition = Plate.TranslatePoint(plateMousePosition, PlateContainer);

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

        private Point? GetBoardCoords(MouseEventArgs? args)
        {
            if (GetPlateCanvasOffset(PlateCanvasContainer) is Thickness canvasOffset && args?.GetPosition(PlateCanvasContainer) is { X: var x, Y: var y })
            {
                return new Point((x - canvasOffset.Left) / UnitSize, (y - canvasOffset.Top) / UnitSize);
            }

            return null;
        }

        private class SelectingContext
        {
            public Point InitialMousePosition { get; init; }

            public Point? InitialCoords { get; init; }

            public KeyContainer? InitialKey { get; init; }

            public List<KeyContainer> Keys { get; init; } = new();
        }

        private class DraggingContext
        {
            public Point InitialMousePosition { get; init; }
        }

        private class KeyContainer
        {
            public KeyContainer(DependencyObject container)
            {
                Container = container;
            }

            public DependencyObject Container { get; }

            public Rect IsBounds
            {
                get => GetBounds(Container);
                private set => SetBounds(Container, value);
            }

            public bool IsPreviewSelected
            {
                get => GetIsPreviewSelected(Container);
                set => SetIsPreviewSelected(Container, value);
            }

            public bool IsSelected
            {
                get => GetIsSelected(Container);
                set => SetIsSelected(Container, value);
            }
        }
    }
}
