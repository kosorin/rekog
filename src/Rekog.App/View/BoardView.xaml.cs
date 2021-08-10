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
        private static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(BoardViewState), typeof(BoardView), new PropertyMetadata(BoardViewState.None));

        public static readonly DependencyProperty SelectingKeysProperty =
            DependencyProperty.Register(nameof(SelectingKeys), typeof(ICommand), typeof(BoardView), new PropertyMetadata(null));

        public static readonly DependencyProperty CanvasOffsetProperty =
            DependencyProperty.RegisterAttached("CanvasOffset", typeof(Thickness), typeof(BoardView), new PropertyMetadata(new Thickness()));

        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.RegisterAttached("Bounds", typeof(Rect), typeof(BoardView), new PropertyMetadata(new Rect()));

        public static readonly DependencyProperty IsPreviewSelectedProperty =
            DependencyProperty.RegisterAttached("IsPreviewSelected", typeof(bool), typeof(BoardView), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(BoardView), new PropertyMetadata(false));

        private readonly double _selectionBoxInflateSize = 0.25;

        private SelectContext? _selectContext;
        private DragContext? _dragContext;

        public BoardView()
        {
            InitializeComponent();
        }

        public BoardViewState State
        {
            get => (BoardViewState)GetValue(StateProperty);
            private set => SetValue(StateProperty, value);
        }

        public ICommand? SelectingKeys
        {
            get => (ICommand?)GetValue(SelectingKeysProperty);
            set => SetValue(SelectingKeysProperty, value);
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


        private List<KeyContainer> GetKeyContainers()
        {
            var list = new List<KeyContainer>();

            foreach (var item in CanvasContainer.Items)
            {
                var container = CanvasContainer.ItemContainerGenerator.ContainerFromItem(item);
                if (container == null)
                {
                    CanvasContainer.UpdateLayout();
                    container = CanvasContainer.ItemContainerGenerator.ContainerFromItem(item);
                }

                if (container?.FindChild<Canvas>(LayoutRoot.Name) is { } canvas)
                {
                    list.Add(new KeyContainer(canvas));
                }
            }

            return list;
        }

        private KeyContainer? GetKeyContainer(MouseEventArgs args)
        {
            if ((args.OriginalSource as DependencyObject)?.FindParent<Canvas>(LayoutRoot.Name) is { } canvas)
            {
                return new KeyContainer(canvas);
            }

            return null;
        }

        private Point GetCoords(MouseEventArgs args)
        {
            var canvasOffset = GetCanvasOffset(CanvasWrapper);
            var position = args.GetPosition(CanvasContainer);
            return new Point((position.X - canvasOffset.Left) / App.UnitSize, (position.Y - canvasOffset.Top) / App.UnitSize);
        }

        private Rect GetCoordBounds()
        {
            var start = LayoutRoot.TranslatePoint(new Point(0, 0), CanvasContainer);
            var end = LayoutRoot.TranslatePoint(new Point(LayoutRoot.ActualWidth, LayoutRoot.ActualHeight), CanvasContainer);
            return new Rect(new Point(start.X / App.UnitSize, start.Y / App.UnitSize),
                new Point(end.X / App.UnitSize, end.Y / App.UnitSize));
        }


        private void UserControl_PreviewMouseDown(object? sender, MouseButtonEventArgs args)
        {
            args.Handled = true;

            if (State != BoardViewState.None)
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
            if (args.LeftButton == MouseButtonState.Pressed && (State == BoardViewState.ClickSelecting || State == BoardViewState.DragSelecting))
            {
                HandleSelect(args);
            }
            else if (args.RightButton == MouseButtonState.Pressed && State == BoardViewState.Dragging)
            {
                HandleDrag(args);
            }
        }

        private void UserControl_PreviewMouseUp(object? sender, MouseButtonEventArgs args)
        {
            args.Handled = true;

            if (State == BoardViewState.None)
            {
                return;
            }

            if (args.ChangedButton == MouseButton.Left && (State == BoardViewState.ClickSelecting || State == BoardViewState.DragSelecting))
            {
                EndSelect(args);
            }
            else if (args.ChangedButton == MouseButton.Right && State == BoardViewState.Dragging)
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

            if (State != BoardViewState.None)
            {
                return;
            }

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                Move(Orientation.Horizontal, args.Delta);
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
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

            if (State != BoardViewState.None)
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

        private void LayoutRoot_SizeChanged(object? sender, SizeChangedEventArgs args)
        {
            Coerce();
        }

        private void Plate_SizeChanged(object? sender, SizeChangedEventArgs args)
        {
            Coerce();
        }


        private void Reset()
        {
            if (State == BoardViewState.None)
            {
                return;
            }

            if (State == BoardViewState.ClickSelecting || State == BoardViewState.DragSelecting)
            {
                CancelSelect();
            }
            else if (State == BoardViewState.Dragging)
            {
                CancelDrag();
            }
            else
            {
                State = BoardViewState.None;
            }

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }


        private void StartSelect(MouseEventArgs args)
        {
            State = BoardViewState.ClickSelecting;

            _selectContext = new SelectContext
            {
                InitialPosition = args.GetPosition(LayoutRoot),
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

            var selectionBox = GetSelectionBox(_selectContext.InitialPosition, args.GetPosition(LayoutRoot), new Rect(new Point(0, 0), LayoutRoot.RenderSize));

            if (State != BoardViewState.DragSelecting)
            {
                if (SystemParameters.MinimumHorizontalDragDistance <= selectionBox.Width || SystemParameters.MinimumVerticalDragDistance <= selectionBox.Height)
                {
                    State = BoardViewState.DragSelecting;

                    ClearSelectionBox();

                    CaptureMouse();
                }
            }

            if (State == BoardViewState.DragSelecting)
            {
                SetSelectionBox(selectionBox);

                PreviewSelectMultiple(args);
            }
        }

        private void EndSelect(MouseEventArgs args)
        {
            if (State == BoardViewState.ClickSelecting)
            {
                SelectSingle(args);

                CancelSelect();
            }
            else if (State == BoardViewState.DragSelecting)
            {
                SelectMultiple(args);

                ReleaseMouseCapture();
            }
        }

        private void CancelSelect()
        {
            if (State == BoardViewState.DragSelecting)
            {
                ClearSelectionBox();

                PreviewUnselectAll();
            }

            _selectContext = null;

            State = BoardViewState.None;
        }

        private void SelectSingle(MouseEventArgs args)
        {
            OnSelectingKeys(true);
            try
            {
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
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
            finally
            {
                OnSelectingKeys(false);
            }
        }

        private void SelectMultiple(MouseEventArgs args)
        {
            OnSelectingKeys(true);
            try
            {
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
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
            finally
            {
                OnSelectingKeys(false);
            }
        }

        private void SelectAll()
        {
            OnSelectingKeys(true);
            try
            {
                foreach (var keyContainer in GetKeyContainers())
                {
                    keyContainer.Select();
                }
            }
            finally
            {
                OnSelectingKeys(false);
            }
        }

        private void UnselectAll()
        {
            OnSelectingKeys(true);
            try
            {
                foreach (var keyContainer in GetKeyContainers())
                {
                    keyContainer.Unselect();
                }
            }
            finally
            {
                OnSelectingKeys(false);
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
                selectionBox = new Rect(selectionBox.X - inflate, selectionBox.Y - inflate,
                    selectionBox.Width + 2 * inflate, selectionBox.Height + 2 * inflate);
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
            State = BoardViewState.Dragging;

            _dragContext = new DragContext
            {
                InitialPosition = PlateMatrixTransform.Inverse!.Transform(args.GetPosition(LayoutRoot)),
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

            State = BoardViewState.None;
        }

        private void Drag(MouseEventArgs args)
        {
            if (_dragContext == null)
            {
                return;
            }

            var matrix = PlateMatrixTransform.Matrix;

            var position = PlateMatrixTransform.Inverse!.Transform(args.GetPosition(LayoutRoot));
            var delta = Point.Subtract(position, _dragContext.InitialPosition);
            matrix.TranslatePrepend(delta.X, delta.Y);

            PlateMatrixTransform.Matrix = matrix;

            Coerce();
        }


        private void Center()
        {
            var matrix = PlateMatrixTransform.Matrix;

            var (layoutRootBounds, plateBounds, plateOffset) = GetBounds();
            matrix.OffsetX = (layoutRootBounds.Width - plateBounds.Width) / 2 - plateOffset.X;
            matrix.OffsetY = (layoutRootBounds.Height - plateBounds.Height) / 2 - plateOffset.Y;

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

            var (layoutRootBounds, plateBounds, plateOffset) = GetBounds();

            var minVisibleSize = new Size(Math.Min(plateBounds.Width, layoutRootBounds.Width),
                Math.Min(plateBounds.Height, layoutRootBounds.Height));

            if (layoutRootBounds.Left + minVisibleSize.Width > plateBounds.Right)
            {
                matrix.OffsetX = minVisibleSize.Width - plateBounds.Width - plateOffset.X;
                coerced = true;
            }
            else if (layoutRootBounds.Right - minVisibleSize.Width < plateBounds.Left)
            {
                matrix.OffsetX = layoutRootBounds.Right - minVisibleSize.Width - plateOffset.X;
                coerced = true;
            }

            if (layoutRootBounds.Top + minVisibleSize.Height > plateBounds.Bottom)
            {
                matrix.OffsetY = minVisibleSize.Height - plateBounds.Height - plateOffset.Y;
                coerced = true;
            }
            else if (layoutRootBounds.Bottom - minVisibleSize.Height < plateBounds.Top)
            {
                matrix.OffsetY = layoutRootBounds.Bottom - minVisibleSize.Height - plateOffset.Y;
                coerced = true;
            }

            if (coerced)
            {
                PlateMatrixTransform.Matrix = matrix;
            }
        }

        private (Rect layoutRootBounds, Rect plateBounds, Point plateOffset) GetBounds()
        {
            var matrix = PlateMatrixTransform.Matrix;
            var scale = matrix.M11;

            var platePosition = new Point(0, 0);
            var layoutRootPosition = Plate.TranslatePoint(platePosition, LayoutRoot);

            var plateBounds = new Rect(new Point(layoutRootPosition.X - platePosition.X * scale, layoutRootPosition.Y - platePosition.Y * scale),
                new Size(Plate.ActualWidth * scale, Plate.ActualHeight * scale));
            var layoutRootBounds = new Rect(new Point(0, 0),
                new Size(LayoutRoot.ActualWidth, LayoutRoot.ActualHeight));

            var plateOffset = new Point(layoutRootPosition.X - (platePosition.X * scale + matrix.OffsetX), layoutRootPosition.Y - (platePosition.Y * scale + matrix.OffsetY));

            return (layoutRootBounds, plateBounds, plateOffset);
        }

        private void OnSelectingKeys(bool selectingKeys)
        {
            if (SelectingKeys is { } command && command.CanExecute(selectingKeys))
            {
                command.Execute(selectingKeys);
            }
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
