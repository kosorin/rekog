using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Koda.ColorTools;
using Koda.ColorTools.Wpf;
using Rekog.App.Extensions;
using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ViewModel.Forms;
using Rekog.App.ViewModel.Values;

namespace Rekog.App.ViewModel
{
    public class BoardViewModel : ViewModelBase<BoardModel>
    {
        private static readonly Color DefaultBackground = Colors.White;

        private ObservableObjectCollection<KeyViewModel> _keys = new ObservableObjectCollection<KeyViewModel>();
        private ObservableObjectCollection<KeyViewModel> _selectedKeys = new ObservableObjectCollection<KeyViewModel>();
        private Thickness _canvasOffset;
        private Size _canvasSize;
        private Color _background = DefaultBackground;

        public BoardViewModel(BoardModel model)
            : base(model)
        {
            AddKeyCommand = new DelegateCommand<NewKeyTemplate>(AddKey);
            DeleteSelectedKeysCommand = new DelegateCommand(DeleteSelectedKeys, CanDeleteSelectedKeys);

            UpdateAll();

            Form.Set(Model);
        }

        public BoardFormViewModel Form { get; } = new BoardFormViewModel();

        public KeyFormViewModel KeyForm { get; } = new KeyFormViewModel();

        public LegendFormViewModel LegendForm { get; } = new LegendFormViewModel();

        public DelegateCommand<NewKeyTemplate> AddKeyCommand { get; }

        public DelegateCommand DeleteSelectedKeysCommand { get; }

        public ObservableObjectCollection<KeyViewModel> Keys
        {
            get => _keys;
            private set
            {
                if (SetCollection<ObservableObjectCollection<KeyViewModel>, KeyViewModel>(ref _keys, value, Keys_CollectionItemChanged, Keys_CollectionItemPropertyChanged))
                {
                    SelectedKeys = new ObservableObjectCollection<KeyViewModel>(Keys.Where(x => x.IsSelected));

                    UpdateCanvas();
                }
            }
        }

        public ObservableObjectCollection<KeyViewModel> SelectedKeys
        {
            get => _selectedKeys;
            private set
            {
                if (SetCollection<ObservableObjectCollection<KeyViewModel>, KeyViewModel>(ref _selectedKeys, value, SelectedKeys_CollectionItemChanged, SelectedKeys_CollectionItemPropertyChanged))
                {
                    UpdateSelectedKeys();

                    foreach (var key in SelectedKeys)
                    {
                        key.IsSelected = true;
                    }
                }
            }
        }

        public PointValueSource SelectedKeysRotationOrigin { get; } = new PointValueSource(new Point());

        public Thickness CanvasOffset
        {
            get => _canvasOffset;
            private set => Set(ref _canvasOffset, value);
        }

        public Size CanvasSize
        {
            get => _canvasSize;
            private set => Set(ref _canvasSize, value);
        }

        public Color Background
        {
            get => _background;
            private set => Set(ref _background, value);
        }

        protected override void OnModelPropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
            base.OnModelPropertyChanging(sender, args);

            switch (args.PropertyName)
            {
                case nameof(BoardModel.Keys):
                    UnsubscribeModelKeys();
                    break;
            }
        }

        protected override void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            base.OnModelPropertyChanged(sender, args);

            switch (args.PropertyName)
            {
                case nameof(BoardModel.Keys):
                    UpdateKeys();
                    SubscribeModelKeys();
                    break;
                case nameof(BoardModel.Background):
                    UpdateBackground();
                    break;
            }
        }

        private void UpdateAll()
        {
            UpdateKeys();
            SubscribeModelKeys();
            UpdateBackground();
        }

        private void UpdateKeys()
        {
            Keys = new ObservableObjectCollection<KeyViewModel>(Model.Keys.Select(x => new KeyViewModel(x)));
        }

        private void UpdateBackground()
        {
            Background = HexColor.TryParse(Model.Background, out var color) ? color.ToColor() : DefaultBackground;
        }

        private void SubscribeModelKeys()
        {
            Model.Keys.CollectionItemChanged += ModelKeys_CollectionItemChanged;
            Model.Keys.CollectionItemPropertyChanged += ModelKeys_CollectionItemPropertyChanged;
        }

        private void UnsubscribeModelKeys()
        {
            Model.Keys.CollectionItemChanged -= ModelKeys_CollectionItemChanged;
            Model.Keys.CollectionItemPropertyChanged -= ModelKeys_CollectionItemPropertyChanged;
        }

        private void ModelKeys_CollectionItemChanged(IObservableObjectCollection<KeyModel> collection, CollectionItemChangedEventArgs<KeyModel> args)
        {
            Keys.RemoveRange(args.OldItems.Join(Keys, model => model, key => key.Model, (_, key) => key));
            Keys.AddRange(args.NewItems.Select(x => new KeyViewModel(x)));
        }

        private void ModelKeys_CollectionItemPropertyChanged(KeyModel item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyModel.RotationAngle):
                case nameof(KeyModel.RotationOriginX):
                case nameof(KeyModel.RotationOriginY):
                    UpdateRotationOrigin(false);
                    break;
            }
        }

        private void Keys_CollectionItemChanged(IObservableObjectCollection<KeyViewModel> collection, CollectionItemChangedEventArgs<KeyViewModel> args)
        {
            if (args.OldItems.Count > 0)
            {
                SelectedKeys.RemoveRange(args.OldItems);
            }

            if (args.NewItems.Count > 0)
            {
                SelectedKeys.MergeRange(args.NewItems);
            }

            UpdateCanvas();
        }

        private void Keys_CollectionItemPropertyChanged(KeyViewModel item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyViewModel.ActualBounds):
                    UpdateCanvas();
                    break;
                case nameof(KeyViewModel.IsSelected):
                    if (item.IsSelected)
                    {
                        SelectedKeys.Merge(item);
                    }
                    break;
            }
        }

        private void SelectedKeys_CollectionItemChanged(IObservableObjectCollection<KeyViewModel> collection, CollectionItemChangedEventArgs<KeyViewModel> args)
        {
            UpdateSelectedKeys();

            foreach (var oldKey in args.OldItems)
            {
                oldKey.IsSelected = false;
            }

            foreach (var newKey in args.NewItems)
            {
                newKey.IsSelected = true;
            }
        }

        private void SelectedKeys_CollectionItemPropertyChanged(KeyViewModel item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyViewModel.IsSelected):
                    if (!item.IsSelected)
                    {
                        SelectedKeys.Remove(item);
                    }
                    break;
            }
        }

        private void UpdateSelectedKeys()
        {
            KeyForm.Set(SelectedKeys.Select(x => x.Model));
            LegendForm.Set(SelectedKeys.Select(x => x.Model.Legends.FirstOrDefault()).NotNull());

            DeleteSelectedKeysCommand.RaiseCanExecuteChanged();

            UpdateRotationOrigin(true);
        }

        private void UpdateRotationOrigin(bool alwaysIncrementVersion)
        {
            if (KeyForm.RotationOriginX.IsSet && KeyForm.RotationOriginY.IsSet &&
                (KeyForm.RotationOriginX.Value != 0 || KeyForm.RotationOriginY.Value != 0 || KeyForm.RotationAngle.IsSet && KeyForm.RotationAngle.Value != 0))
            {
                if (alwaysIncrementVersion || !SelectedKeysRotationOrigin.IsSet)
                {
                    SelectedKeysRotationOrigin.Version++;
                }
                SelectedKeysRotationOrigin.Value = new Point(KeyForm.RotationOriginX.Value!.Value, KeyForm.RotationOriginY.Value!.Value);
                SelectedKeysRotationOrigin.IsSet = true;
            }
            else
            {
                SelectedKeysRotationOrigin.IsSet = false;
            }
        }

        private void UpdateCanvas()
        {
            if (!Keys.Any())
            {
                CanvasOffset = new Thickness();
                CanvasSize = new Size();
                return;
            }

            var left = Keys.Min(x => x.ActualBounds.Left);
            var top = Keys.Min(x => x.ActualBounds.Top);
            var right = Keys.Max(x => x.ActualBounds.Right);
            var bottom = Keys.Max(x => x.ActualBounds.Bottom);

            CanvasOffset = new Thickness(-left, -top, left, top);
            CanvasSize = new Size(right - left, bottom - top);
        }

        private void AddKey(NewKeyTemplate template)
        {
            var y = Keys.OrderByDescending(x => x.ActualBounds.Bottom).FirstOrDefault()?.ActualBounds.Bottom ?? 0;
            var keyModel = template switch
            {
                NewKeyTemplate.IsoEnter => new KeyModel
                {
                    Y = y,
                    Width = 1.25,
                    Height = 2.0,
                    UseShape = true,
                    Shape = "M-.25 0 0 0 1.25 0 1.25 1 1.25 2 0 2 0 1-.25 1-.25 0z",
                    SteppedWidth = 1.25,
                    SteppedHeight = 2.0,
                },
                NewKeyTemplate.BigAssEnter => new KeyModel
                {
                    Y = y,
                    Width = 1.5,
                    Height = 2.0,
                    UseShape = true,
                    Shape = "M0 0 1.5 0 1.5 1 1.5 2 0 2-.75 2-.75 1 0 1 0 0z",
                    SteppedWidth = 1.5,
                    SteppedHeight = 2.0,
                },
                NewKeyTemplate.SteppedCapsLock => new KeyModel
                {
                    Y = y,
                    Width = 1.75,
                    Height = 1,
                    IsStepped = true,
                    SteppedWidth = 1.25,
                    SteppedHeight = 1,
                },
                NewKeyTemplate.CenterStepped => new KeyModel
                {
                    Y = y,
                    Width = 1.55,
                    Height = 1,
                    IsStepped = true,
                    SteppedOffsetX = 0.25,
                    SteppedOffsetY = 0,
                    SteppedWidth = 1,
                    SteppedHeight = 1,
                },
                _ => new KeyModel
                {
                    Y = y,
                },
            };

            Model.Keys.Add(keyModel);

            SelectKeyModel(keyModel);
        }

        private void SelectKeyModel(KeyModel model)
        {
            if (Keys.FirstOrDefault(x => x.Model == model) is { } key)
            {
                SelectedKeys.ReplaceUsingClear(new[] { key, });
            }
        }

        private void DeleteSelectedKeys()
        {
            Model.Keys.RemoveRange(SelectedKeys.Select(x => x.Model));
        }

        private bool CanDeleteSelectedKeys()
        {
            return SelectedKeys.Any();
        }
    }
}
