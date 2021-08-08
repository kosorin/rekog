using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rekog.App.Extensions;

namespace Rekog.App.View
{
    public partial class BoardView
    {
        public static readonly DependencyProperty CanvasOffsetProperty =
            DependencyProperty.RegisterAttached("CanvasOffset", typeof(Thickness), typeof(BoardView), new PropertyMetadata(new Thickness()));

        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.RegisterAttached("Bounds", typeof(Rect), typeof(BoardView), new PropertyMetadata(new Rect()));

        public static readonly DependencyProperty IsPreviewSelectedProperty =
            DependencyProperty.RegisterAttached("IsPreviewSelected", typeof(bool), typeof(BoardView), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(BoardView), new PropertyMetadata(false));

        private static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(BoardState), typeof(BoardView), new PropertyMetadata(BoardState.None));

        private readonly double _selectionBoxInflateSize = 0.25;

        private SelectContext? _selectContext;
        private DragContext? _dragContext;

        public BoardView()
        {
            InitializeComponent();
        }

        public BoardState State
        {
            get => (BoardState)GetValue(StateProperty);
            private set => SetValue(StateProperty, value);
        }


        public static Thickness GetCanvasOffset(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(CanvasOffsetProperty);
        }

        public static void SetCanvasOffset(DependencyObject obj, Thickness value)
        {
            obj.SetValue(CanvasOffsetProperty, value);
        }

        public static Rect GetBounds(DependencyObject obj)
        {
            return (Rect)obj.GetValue(BoundsProperty);
        }

        public static void SetBounds(DependencyObject obj, Rect value)
        {
            obj.SetValue(BoundsProperty, value);
        }

        public static bool GetIsPreviewSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPreviewSelectedProperty);
        }

        public static void SetIsPreviewSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPreviewSelectedProperty, value);
        }

        public static bool GetIsSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedProperty, value);
        }


        private static bool IsShift()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        private static bool IsCtrl()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

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

                if (container?.FindChild<Canvas>("Root") is { } canvas)
                {
                    list.Add(new KeyContainer(canvas));
                }
            }

            return list;
        }

        private KeyContainer? GetKeyContainer(MouseEventArgs args)
        {
            if ((args.OriginalSource as DependencyObject)?.FindParent<Canvas>("Root") is { } canvas)
            {
                return new KeyContainer(canvas);
            }

            return null;
        }

        private Point GetCoords(MouseEventArgs args)
        {
            var canvasOffset = GetCanvasOffset(CanvasContainer);
            var position = args.GetPosition(PlateCanvasContainer);
            return new Point((position.X - canvasOffset.Left) / App.UnitSize, (position.Y - canvasOffset.Top) / App.UnitSize);
        }

        private Rect GetCoordBounds()
        {
            var start = PlateContainer.TranslatePoint(new Point(0, 0), PlateCanvasContainer);
            var end = PlateContainer.TranslatePoint(new Point(PlateContainer.ActualWidth, PlateContainer.ActualHeight), PlateCanvasContainer);
            return new Rect(new Point(start.X / App.UnitSize, start.Y / App.UnitSize),
                new Point(end.X / App.UnitSize, end.Y / App.UnitSize));
        }


        private void UserControl_PreviewMouseDown(object? sender, MouseButtonEventArgs args)
        {
            args.Handled = true;

            if (State != BoardState.None)
            {
                return;
            }

            Focus();

            if (args.ChangedButton == MouseButton.Left)
            {
                StartSelect(args);
            }
            else if (args.ChangedButton == MouseButton.Right)
            {
                StartDrag(args);
            }
            else if (args.ChangedButton == MouseButton.Middle)
            {
                Center();
            }
        }

        private void UserControl_PreviewMouseMove(object? sender, MouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed && (State == BoardState.ClickSelecting || State == BoardState.DragSelecting))
            {
                HandleSelect(args);
            }
            else if (args.RightButton == MouseButtonState.Pressed && State == BoardState.Dragging)
            {
                HandleDrag(args);
            }
        }

        private void UserControl_PreviewMouseUp(object? sender, MouseButtonEventArgs args)
        {
            args.Handled = true;

            if (State == BoardState.None)
            {
                return;
            }

            if (args.ChangedButton == MouseButton.Left && (State == BoardState.ClickSelecting || State == BoardState.DragSelecting))
            {
                EndSelect(args);
            }
            else if (args.ChangedButton == MouseButton.Right && State == BoardState.Dragging)
            {
                EndDrag(args);
            }
        }

        private void UserControl_LostMouseCapture(object? sender, MouseEventArgs args)
        {
            Reset();
        }

        private void UserControl_PreviewMouseWheel(object? sender, MouseWheelEventArgs args)
        {
            args.Handled = true;

            if (State != BoardState.None)
            {
                return;
            }

            if (IsShift())
            {
                Move(Orientation.Horizontal, args.Delta);
            }
            else if (IsCtrl())
            {
                Move(Orientation.Vertical, args.Delta);
            }
            else
            {
                Zoom(args.GetPosition(Plate), args.Delta);
            }
        }

        private void UserControl_PreviewKeyDown(object? sender, KeyEventArgs args)
        {
            args.Handled = true;

            if (State != BoardState.None)
            {
                switch (args.Key)
                {
                    case Key.Escape:
                        Reset();
                        break;
                    default:
                        args.Handled = false;
                        break;
                }
            }
            else
            {
                switch (args.Key)
                {
                    case Key.A when Keyboard.Modifiers.HasFlag(ModifierKeys.Control):
                        SelectAll();
                        break;
                    case Key.Left:
                        Move(Orientation.Horizontal, -1);
                        break;
                    case Key.Right:
                        Move(Orientation.Horizontal, 1);
                        break;
                    case Key.Up:
                        Move(Orientation.Vertical, -1);
                        break;
                    case Key.Down:
                        Move(Orientation.Vertical, 1);
                        break;
                    default:
                        args.Handled = false;
                        break;
                }
            }
        }

        private void PlateContainer_SizeChanged(object? sender, SizeChangedEventArgs args)
        {
            Coerce();
        }

        private void Plate_SizeChanged(object? sender, SizeChangedEventArgs args)
        {
            Coerce();
        }


        private void Reset()
        {
            if (State == BoardState.None)
            {
                return;
            }

            if (State == BoardState.ClickSelecting || State == BoardState.DragSelecting)
            {
                CancelSelect();
            }
            else if (State == BoardState.Dragging)
            {
                CancelDrag();
            }
            else
            {
                State = BoardState.None;
            }

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }


        private void StartSelect(MouseEventArgs args)
        {
            State = BoardState.ClickSelecting;

            _selectContext = new SelectContext
            {
                InitialPosition = args.GetPosition(PlateContainer),
                InitialCoords = GetCoords(args),
                InitialKey = GetKeyContainer(args),
                Keys = GetKeyContainers(),
            };
        }

        private void HandleSelect(MouseEventArgs args)
        {
            if (_selectContext == null)
            {
                return;
            }

            var selectionBox = GetSelectionBox(_selectContext.InitialPosition, args.GetPosition(PlateContainer), new Rect(new Point(0, 0), PlateContainer.RenderSize));

            if (State != BoardState.DragSelecting)
            {
                if (SystemParameters.MinimumHorizontalDragDistance <= selectionBox.Width || SystemParameters.MinimumVerticalDragDistance <= selectionBox.Height)
                {
                    State = BoardState.DragSelecting;

                    ClearSelectionBox();

                    CaptureMouse();
                }
            }

            if (State == BoardState.DragSelecting)
            {
                SetSelectionBox(selectionBox);

                PreviewSelectMultiple(args);
            }
        }

        private void EndSelect(MouseEventArgs args)
        {
            if (State == BoardState.ClickSelecting)
            {
                SelectSingle(args);

                CancelSelect();
            }
            else if (State == BoardState.DragSelecting)
            {
                SelectMultiple(args);

                ReleaseMouseCapture();
            }
        }

        private void CancelSelect()
        {
            if (State == BoardState.DragSelecting)
            {
                ClearSelectionBox();

                PreviewUnselectAll();
            }

            _selectContext = null;

            State = BoardState.None;
        }

        private void SelectSingle(MouseEventArgs args)
        {
            if (!IsCtrl())
            {
                UnselectAll();
            }

            if (_selectContext == null)
            {
                return;
            }

            if (_selectContext.InitialKey is not { } initialKey || GetKeyContainer(args) is not { } key || initialKey.Container != key.Container)
            {
                return;
            }

            key.Toggle();
        }

        private void SelectMultiple(MouseEventArgs args)
        {
            if (!IsCtrl())
            {
                UnselectAll();
            }

            if (_selectContext == null)
            {
                return;
            }

            var selectionBox = GetSelectionBox(_selectContext.InitialCoords, GetCoords(args), GetCoordBounds(), _selectionBoxInflateSize);

            foreach (var keyContainer in _selectContext.Keys.Where(x => selectionBox.Contains(x.Bounds)))
            {
                keyContainer.Select();
            }
        }

        private void SelectAll()
        {
            foreach (var keyContainer in GetKeyContainers())
            {
                keyContainer.Select();
            }
        }

        private void UnselectAll()
        {
            foreach (var keyContainer in GetKeyContainers())
            {
                keyContainer.Unselect();
            }
        }

        private void PreviewSelectMultiple(MouseEventArgs args)
        {
            if (_selectContext == null)
            {
                return;
            }

            var selectionBox = GetSelectionBox(_selectContext.InitialCoords, GetCoords(args), GetCoordBounds(), _selectionBoxInflateSize);

            foreach (var keyContainer in _selectContext.Keys)
            {
                if (selectionBox.Contains(keyContainer.Bounds))
                {
                    keyContainer.PreviewSelect();
                }
                else
                {
                    keyContainer.PreviewUnselect();
                }
            }
        }

        private void PreviewUnselectAll()
        {
            if (_selectContext == null)
            {
                return;
            }

            foreach (var keyContainer in _selectContext.Keys)
            {
                keyContainer.PreviewUnselect();
            }
        }

        private Rect GetSelectionBox(Point start, Point end, Rect bounds, double inflate = 0)
        {
            var selectionBox = new Rect(start, end);

            selectionBox.Intersect(bounds);

            // TODO: Fix bug when selectionBox is Empty after intersect

            if (inflate != 0)
            {
                selectionBox = new Rect(selectionBox.X - inflate,
                    selectionBox.Y - inflate,
                    selectionBox.Width + 2 * inflate,
                    selectionBox.Height + 2 * inflate);
            }

            return selectionBox;
        }

        private void SetSelectionBox(Rect selectionBox)
        {
            Canvas.SetLeft(SelectionBox, selectionBox.X);
            Canvas.SetTop(SelectionBox, selectionBox.Y);
            SelectionBox.Width = selectionBox.Width;
            SelectionBox.Height = selectionBox.Height;
        }

        private void ClearSelectionBox()
        {
            SelectionBox.Width = 0;
            SelectionBox.Height = 0;
        }


        private void StartDrag(MouseEventArgs args)
        {
            State = BoardState.Dragging;

            _dragContext = new DragContext
            {
                InitialPosition = PlateMatrixTransform.Inverse!.Transform(args.GetPosition(PlateContainer)),
            };

            CaptureMouse();
        }

        private void HandleDrag(MouseEventArgs args)
        {
            Drag(args);
        }

        private void EndDrag(MouseEventArgs args)
        {
            Drag(args);

            ReleaseMouseCapture();
        }

        private void CancelDrag()
        {
            _dragContext = null;

            State = BoardState.None;
        }

        private void Drag(MouseEventArgs args)
        {
            if (_dragContext == null)
            {
                return;
            }

            var matrix = PlateMatrixTransform.Matrix;

            var position = PlateMatrixTransform.Inverse!.Transform(args.GetPosition(PlateContainer));
            var delta = Point.Subtract(position, _dragContext.InitialPosition);
            matrix.TranslatePrepend(delta.X, delta.Y);

            PlateMatrixTransform.Matrix = matrix;

            Coerce();
        }


        private void Center()
        {
            var matrix = PlateMatrixTransform.Matrix;

            var (plateContainerBounds, plateBounds, plateOffset) = GetBounds();
            matrix.OffsetX = (plateContainerBounds.Width - plateBounds.Width) / 2 - plateOffset.X;
            matrix.OffsetY = (plateContainerBounds.Height - plateBounds.Height) / 2 - plateOffset.Y;

            PlateMatrixTransform.Matrix = matrix;
        }

        private void Move(Orientation orientation, int sign)
        {
            if (sign == 0)
            {
                return;
            }

            var matrix = PlateMatrixTransform.Matrix;

            var delta = App.UnitSize * 0.5 * Math.Sign(sign) / matrix.M11;
            switch (orientation)
            {
                case Orientation.Horizontal:
                    matrix.TranslatePrepend(delta, 0);
                    break;
                case Orientation.Vertical:
                    matrix.TranslatePrepend(0, delta);
                    break;
            }

            PlateMatrixTransform.Matrix = matrix;

            Coerce();
        }

        private void Zoom(Point center, int delta)
        {
            var matrix = PlateMatrixTransform.Matrix;

            var factor = 1.2;
            var scale = delta >= 0 ? factor : 1.0 / factor;

            matrix.ScaleAtPrepend(scale, scale, center.X, center.Y);
            if (matrix.M11 is < 0.2 or > 5.0)
            {
                return;
            }

            PlateMatrixTransform.Matrix = matrix;

            Coerce();
        }

        private void Coerce()
        {
            var coerced = false;
            var matrix = PlateMatrixTransform.Matrix;

            var (plateContainerBounds, plateBounds, plateOffset) = GetBounds();

            var minVisibleSize = new Size(Math.Min(plateBounds.Width, plateContainerBounds.Width),
                Math.Min(plateBounds.Height, plateContainerBounds.Height));

            if (plateContainerBounds.Left + minVisibleSize.Width > plateBounds.Right)
            {
                matrix.OffsetX = minVisibleSize.Width - plateBounds.Width - plateOffset.X;
                coerced = true;
            }
            else if (plateContainerBounds.Right - minVisibleSize.Width < plateBounds.Left)
            {
                matrix.OffsetX = plateContainerBounds.Right - minVisibleSize.Width - plateOffset.X;
                coerced = true;
            }

            if (plateContainerBounds.Top + minVisibleSize.Height > plateBounds.Bottom)
            {
                matrix.OffsetY = minVisibleSize.Height - plateBounds.Height - plateOffset.Y;
                coerced = true;
            }
            else if (plateContainerBounds.Bottom - minVisibleSize.Height < plateBounds.Top)
            {
                matrix.OffsetY = plateContainerBounds.Bottom - minVisibleSize.Height - plateOffset.Y;
                coerced = true;
            }

            if (coerced)
            {
                PlateMatrixTransform.Matrix = matrix;
            }
        }

        private (Rect plateContainerBounds, Rect plateBounds, Point plateOffset) GetBounds()
        {
            var matrix = PlateMatrixTransform.Matrix;
            var scale = matrix.M11;

            var platePosition = new Point(0, 0);
            var plateContainerPosition = Plate.TranslatePoint(platePosition, PlateContainer);

            var plateBounds = new Rect(new Point(plateContainerPosition.X - platePosition.X * scale,
                    plateContainerPosition.Y - platePosition.Y * scale),
                new Size(Plate.ActualWidth * scale,
                    Plate.ActualHeight * scale));
            var plateContainerBounds = new Rect(new Point(0, 0),
                new Size(PlateContainer.ActualWidth, PlateContainer.ActualHeight));

            var plateOffset = new Point(plateContainerPosition.X - (platePosition.X * scale + matrix.OffsetX),
                plateContainerPosition.Y - (platePosition.Y * scale + matrix.OffsetY));

            return (plateContainerBounds, plateBounds, plateOffset);
        }

        private class SelectContext
        {
            public Point InitialPosition { get; init; }

            public Point InitialCoords { get; init; }

            public KeyContainer? InitialKey { get; init; }

            public List<KeyContainer> Keys { get; init; } = new List<KeyContainer>();
        }

        private class DragContext
        {
            public Point InitialPosition { get; init; }
        }

        private class KeyContainer
        {
            public KeyContainer(DependencyObject container)
            {
                Container = container;
            }

            public DependencyObject Container { get; }

            public Rect Bounds => GetBounds(Container);

            public void PreviewSelect()
            {
                SetIsPreviewSelected(Container, true);
            }

            public void PreviewUnselect()
            {
                SetIsPreviewSelected(Container, false);
            }

            public void Select()
            {
                SetIsSelected(Container, true);
            }

            public void Unselect()
            {
                SetIsSelected(Container, false);
            }

            public void Toggle()
            {
                SetIsSelected(Container, GetIsSelected(Container) ^ true);
            }
        }
    }
}
