using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rekog.App.Extensions;
using Rekog.App.Model;
using Rekog.App.ViewModel;

namespace Rekog.App.View
{
    public partial class BoardView
    {
        private static readonly DependencyPropertyKey CoordsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Coords), typeof(Point), typeof(BoardView), new PropertyMetadata(default(Point)));

        private static readonly DependencyPropertyKey StatePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(State), typeof(BoardViewState), typeof(BoardView), new PropertyMetadata(BoardViewState.None));

        public static readonly DependencyProperty CoordsProperty = CoordsPropertyKey.DependencyProperty;

        public static readonly DependencyProperty StateProperty = StatePropertyKey.DependencyProperty;

        public static readonly DependencyProperty SelectionBoxInflateSizeProperty =
            DependencyProperty.Register(nameof(SelectionBoxInflateSize), typeof(double), typeof(BoardView), new PropertyMetadata(0.25));

        private Canvas? _canvas;
        private SelectContext? _selectContext;
        private DragContext? _dragContext;

        public BoardView()
        {
            InitializeComponent();
        }

        public BoardViewModel ViewModel => (BoardViewModel)DataContext;

        public Point Coords
        {
            get => (Point)GetValue(CoordsProperty);
            private set => SetValue(CoordsPropertyKey, value);
        }

        public BoardViewState State
        {
            get => (BoardViewState)GetValue(StateProperty);
            private set => SetValue(StatePropertyKey, value);
        }

        public double SelectionBoxInflateSize
        {
            get => (double)GetValue(SelectionBoxInflateSizeProperty);
            set => SetValue(SelectionBoxInflateSizeProperty, value);
        }

        [MemberNotNull(nameof(_canvas))]
        private void EnsureCanvas()
        {
            if (_canvas != null)
            {
                return;
            }

            var itemsPresenter = CanvasContainer.FindChild<ItemsPresenter>() ?? throw new NullReferenceException();
            _canvas = itemsPresenter.FindChild<Canvas>() ?? throw new NullReferenceException();
        }

        /// <param name="position">Position relative to <see cref="CanvasContainer" />.</param>
        private Point GetCoords(Point position)
        {
            EnsureCanvas();
            var canvasOffset = _canvas.Margin;
            return new Point((position.X - canvasOffset.Left) / App.UnitSize, (position.Y - canvasOffset.Top) / App.UnitSize);
        }

        private Point GetCoords(MouseEventArgs args)
        {
            var position = args.GetPosition(CanvasContainer);
            return GetCoords(position);
        }

        private Rect GetCoordBounds()
        {
            var start = LayoutRoot.TranslatePoint(new Point(0, 0), CanvasContainer);
            var end = LayoutRoot.TranslatePoint(new Point(LayoutRoot.ActualWidth, LayoutRoot.ActualHeight), CanvasContainer);
            return new Rect(GetCoords(start), GetCoords(end));
        }

        private KeyView? GetKey(MouseEventArgs args)
        {
            return (args.OriginalSource as DependencyObject)?.FindParent<KeyView>();
        }

        private List<KeyView> GetKeys()
        {
            var list = new List<KeyView>();

            // TODO: Cache items
            foreach (var item in CanvasContainer.Items)
            {
                var container = CanvasContainer.ItemContainerGenerator.ContainerFromItem(item);
                if (container == null)
                {
                    CanvasContainer.UpdateLayout();
                    container = CanvasContainer.ItemContainerGenerator.ContainerFromItem(item);
                }

                if (container?.FindChild<KeyView>() is { } key)
                {
                    list.Add(key);
                }
            }

            return list;
        }

        private Rect ClipArea(Point start, Point end, Rect bounds, double inflate)
        {
            var selectionBox = new Rect(start, end);

            selectionBox.Intersect(bounds);

            if (selectionBox != Rect.Empty && inflate != 0)
            {
                selectionBox.Inflate(inflate, inflate);
            }

            return selectionBox;
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

        private void OnBoardViewPreviewMouseDown(object? sender, MouseButtonEventArgs args)
        {
            args.Handled = true;
            Coords = GetCoords(args);

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
        }

        private void OnBoardViewPreviewMouseMove(object? sender, MouseEventArgs args)
        {
            Coords = GetCoords(args);

            if (args.LeftButton == MouseButtonState.Pressed && (State == BoardViewState.ClickSelecting || State == BoardViewState.DragSelecting))
            {
                HandleSelect(args);
            }
            else if (args.RightButton == MouseButtonState.Pressed && State == BoardViewState.Dragging)
            {
                HandleDrag(args);
            }
        }

        private void OnBoardViewPreviewMouseUp(object? sender, MouseButtonEventArgs args)
        {
            args.Handled = true;
            Coords = GetCoords(args);

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

        private void OnBoardViewLostMouseCapture(object? sender, MouseEventArgs args)
        {
            Reset();
        }

        private void OnBoardViewPreviewMouseWheel(object? sender, MouseWheelEventArgs args)
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

        private void OnBoardViewPreviewKeyDown(object? sender, KeyEventArgs args)
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
                    case Key.Escape:
                        UnselectAll();
                        break;
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
                    case Key.Space:
                        Center();
                        break;
                    case Key.Delete:
                        ViewModel.DeleteSelectedKeys();
                        break;
                    case Key.Insert:
                        ViewModel.AddKey(NewKeyTemplate.None);
                        break;
                    default:
                        args.Handled = false;
                        break;
                }
            }
        }

        private void OnLayoutRootSizeChanged(object? sender, SizeChangedEventArgs args)
        {
            Coerce();
        }

        private void OnPlateSizeChanged(object? sender, SizeChangedEventArgs args)
        {
            Coerce();
        }

        #region Select

        private void StartSelect(MouseEventArgs args)
        {
            State = BoardViewState.ClickSelecting;

            _selectContext = new SelectContext
            {
                InitialPosition = args.GetPosition(LayoutRoot),
                InitialCoords = GetCoords(args),
                InitialKey = GetKey(args),
                Keys = GetKeys(),
            };
        }

        private void HandleSelect(MouseEventArgs args)
        {
            if (_selectContext == null)
            {
                return;
            }

            var selectionBox = ClipArea(_selectContext.InitialPosition, args.GetPosition(LayoutRoot), new Rect(new Point(0, 0), LayoutRoot.RenderSize), 0);

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

                PreviewSelectMany(args);
            }
        }

        private void EndSelect(MouseEventArgs args)
        {
            if (State == BoardViewState.ClickSelecting)
            {
                SelectOne(args);

                CancelSelect();
            }
            else if (State == BoardViewState.DragSelecting)
            {
                SelectMany(args);

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

        private void SelectOne(MouseEventArgs args)
        {
            if (_selectContext == null)
            {
                return;
            }

            if (_selectContext.InitialKey is not { } initialKey || GetKey(args) is not { ViewModel: { } keyViewModel, } || initialKey.ViewModel != keyViewModel)
            {
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    UnselectAll();
                }
                return;
            }

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (ViewModel.SelectedKeys.ContainsKey(keyViewModel.Model.Id))
                {
                    ViewModel.SelectedKeys.Remove(keyViewModel.Model.Id);
                }
                else
                {
                    ViewModel.SelectedKeys.Add(keyViewModel.Model.Id, keyViewModel);
                }
            }
            else
            {
                ViewModel.SelectedKeys.ClearOverwrite(new[] { new KeyValuePair<KeyId, KeyViewModel>(keyViewModel.Model.Id, keyViewModel), });
            }
        }

        private void SelectMany(MouseEventArgs args)
        {
            if (_selectContext == null)
            {
                return;
            }

            var selectionBox = ClipArea(_selectContext.InitialCoords, GetCoords(args), GetCoordBounds(), SelectionBoxInflateSize);
            var toSelect = _selectContext.Keys
                .Where(x => selectionBox.Contains(x.ViewModel.ActualBounds))
                .ToDictionary(x => x.ViewModel.Model.Id, x => x.ViewModel);

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ViewModel.SelectedKeys.AddOrReplaceRange(toSelect);
            }
            else
            {
                ViewModel.SelectedKeys.MergeOverwrite(toSelect);
            }
        }

        private void SelectAll()
        {
            ViewModel.SelectedKeys.AddOrReplaceRange(ViewModel.Keys.ToDictionary(x => x.Model.Id, x => x));
        }

        private void UnselectAll()
        {
            ViewModel.SelectedKeys.Clear();
        }

        private void PreviewSelectMany(MouseEventArgs args)
        {
            if (_selectContext == null)
            {
                return;
            }

            var selectionBox = ClipArea(_selectContext.InitialCoords, GetCoords(args), GetCoordBounds(), SelectionBoxInflateSize);

            foreach (var key in _selectContext.Keys)
            {
                key.IsPreviewSelected = selectionBox.Contains(key.ViewModel.ActualBounds);
            }
        }

        private void PreviewUnselectAll()
        {
            if (_selectContext == null)
            {
                return;
            }

            foreach (var key in _selectContext.Keys)
            {
                key.IsPreviewSelected = false;
            }
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

        private class SelectContext
        {
            public Point InitialPosition { get; init; }

            public Point InitialCoords { get; init; }

            public KeyView? InitialKey { get; init; }

            public List<KeyView> Keys { get; init; } = new List<KeyView>();
        }

        #endregion

        #region Drag

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

        private class DragContext
        {
            public Point InitialPosition { get; init; }
        }

        #endregion

        #region View manipulation

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

        #endregion
    }
}
