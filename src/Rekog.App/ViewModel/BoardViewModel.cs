using System.Collections.ObjectModel;
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
using Rekog.App.ViewModel.Forms.Tabs;
using Rekog.App.ViewModel.Values;

namespace Rekog.App.ViewModel
{
    public class BoardViewModel : ViewModelBase<BoardModel>
    {
        private static readonly Color DefaultBackground = Colors.White;

        private ViewModelBase? _currentForm;

        // Each item is in the collection only once.
        private readonly ObservableObjectCollection<FormTabViewModel> _tabs = new ObservableObjectCollection<FormTabViewModel>();
        private readonly ObservableObjectCollection<LayerViewModel> _layers = new ObservableObjectCollection<LayerViewModel>();
        private readonly ObservableObjectCollection<KeyViewModel> _keys = new ObservableObjectCollection<KeyViewModel>();

        // The selected collections are subsets of the respective collections. 
        // IsSelected properties are for caching, however can be used to select items.
        private readonly ObservableObjectCollection<FormTabViewModel> _selectedTabs = new ObservableObjectCollection<FormTabViewModel>();
        private readonly ObservableObjectCollection<LayerViewModel> _selectedLayers = new ObservableObjectCollection<LayerViewModel>();
        private readonly ObservableObjectCollection<KeyViewModel> _selectedKeys = new ObservableObjectCollection<KeyViewModel>();

        private readonly FileFormViewModel _fileForm = new FileFormViewModel();
        private readonly PropertiesFormViewModel _propertiesForm = new PropertiesFormViewModel();
        private readonly BoardFormViewModel _boardForm = new BoardFormViewModel();
        private readonly KeyFormViewModel _keyForm = new KeyFormViewModel();
        private readonly LegendFormViewModel _legendForm = new LegendFormViewModel();

        private readonly FormTabViewModel _fileTab;
        private readonly FormTabViewModel _propertiesTab;
        private readonly FormTabViewModel _boardTab;
        private readonly FormTabViewModel _keyTab;

        private Thickness _canvasOffset;
        private Size _canvasSize;
        private Color _background = DefaultBackground;

        public BoardViewModel(BoardModel model)
            : base(model)
        {
            SubscribeModelLayers();
            SubscribeModelKeys();

            _tabs.Subscribe<ObservableObjectCollection<FormTabViewModel>, FormTabViewModel>(Tabs_CollectionItemChanged, Tabs_CollectionItemPropertyChanged);
            _layers.Subscribe<ObservableObjectCollection<LayerViewModel>, LayerViewModel>(Layers_CollectionItemChanged, Layers_CollectionItemPropertyChanged);
            _keys.Subscribe<ObservableObjectCollection<KeyViewModel>, KeyViewModel>(Keys_CollectionItemChanged, Keys_CollectionItemPropertyChanged);
            _selectedTabs.Subscribe<ObservableObjectCollection<FormTabViewModel>, FormTabViewModel>(SelectedTabs_CollectionItemChanged, SelectedTabs_CollectionItemPropertyChanged);
            _selectedLayers.Subscribe<ObservableObjectCollection<LayerViewModel>, LayerViewModel>(SelectedLayers_CollectionItemChanged, SelectedLayers_CollectionItemPropertyChanged);
            _selectedKeys.Subscribe<ObservableObjectCollection<KeyViewModel>, KeyViewModel>(SelectedKeys_CollectionItemChanged, SelectedKeys_CollectionItemPropertyChanged);

            Tabs = new ReadOnlyObservableCollection<FormTabViewModel>(_tabs);
            Layers = new ReadOnlyObservableCollection<LayerViewModel>(_layers);
            Keys = new ReadOnlyObservableCollection<KeyViewModel>(_keys);
            SelectedTabs = _selectedTabs;
            SelectedLayers = _selectedLayers;
            SelectedKeys = _selectedKeys;

            _fileForm.Set(Model);
            _propertiesForm.Set(Model);
            _boardForm.Set(Model);
            _keyForm.Clear();
            _legendForm.Clear();

            GeneralTabs.AddRange(new[]
            {
                _fileTab = new FormTabViewModel("File", "\uE130", _fileForm) { IsSelected = true, },
                _propertiesTab = new FormTabViewModel("Properties", "\uE946", _propertiesForm),
            });
            LayoutTabs.AddRange(new[]
            {
                _boardTab = new FormTabViewModel("Board", "\uE809", _boardForm),
                _keyTab = new FormTabViewModel("Key", "\uF158", _keyForm),
            });
            _tabs.AddRange(GeneralTabs.Concat(LayoutTabs));

            UpdateLayers();
            UpdateKeys();
            UpdateBackground();

            AddLayerCommand = new DelegateCommand(AddLayer);
            DeleteLayerCommand = new DelegateCommand<LayerModel>(DeleteLayer);
            SelectAllLayersCommand = new DelegateCommand(SelectAllLayers);
            AddKeyCommand = new DelegateCommand<NewKeyTemplate>(AddKey);
            DeleteSelectedKeysCommand = new DelegateCommand(DeleteSelectedKeys, CanDeleteSelectedKeys);
        }

        public DelegateCommand AddLayerCommand { get; }

        public DelegateCommand<LayerModel> DeleteLayerCommand { get; }

        public DelegateCommand SelectAllLayersCommand { get; }

        public DelegateCommand<NewKeyTemplate> AddKeyCommand { get; }

        public DelegateCommand DeleteSelectedKeysCommand { get; }

        public ViewModelBase? CurrentForm
        {
            get => _currentForm;
            private set => Set(ref _currentForm, value);
        }

        public ObservableObjectCollection<FormTabViewModel> GeneralTabs { get; } = new ObservableObjectCollection<FormTabViewModel>();

        public ObservableObjectCollection<FormTabViewModel> LayoutTabs { get; } = new ObservableObjectCollection<FormTabViewModel>();

        public ObservableObjectCollection<LayerFormTabViewModel> LayerTabs { get; } = new ObservableObjectCollection<LayerFormTabViewModel>();

        public ReadOnlyObservableCollection<FormTabViewModel> Tabs { get; }

        public ReadOnlyObservableCollection<LayerViewModel> Layers { get; }

        public ReadOnlyObservableCollection<KeyViewModel> Keys { get; }

        public ObservableObjectCollection<FormTabViewModel> SelectedTabs { get; }

        public ObservableObjectCollection<LayerViewModel> SelectedLayers { get; }

        public ObservableObjectCollection<KeyViewModel> SelectedKeys { get; }

        // TODO: Rework
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
                case nameof(BoardModel.Layers):
                    UnsubscribeModelLayers();
                    break;
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
                case nameof(BoardModel.Layers):
                    UpdateLayers();
                    SubscribeModelLayers();
                    break;
                case nameof(BoardModel.Keys):
                    UpdateKeys();
                    SubscribeModelKeys();
                    break;
                case nameof(BoardModel.Background):
                    UpdateBackground();
                    break;
            }
        }

        private void SubscribeModelLayers()
        {
            UnsubscribeModelLayers();

            Model.Layers.CollectionItemChanged += ModelLayers_CollectionItemChanged;
            Model.Layers.CollectionItemPropertyChanged += ModelLayers_CollectionItemPropertyChanged;
        }

        private void UnsubscribeModelLayers()
        {
            Model.Layers.CollectionItemChanged -= ModelLayers_CollectionItemChanged;
            Model.Layers.CollectionItemPropertyChanged -= ModelLayers_CollectionItemPropertyChanged;
        }

        private void SubscribeModelKeys()
        {
            UnsubscribeModelKeys();

            Model.Keys.CollectionItemChanged += ModelKeys_CollectionItemChanged;
            Model.Keys.CollectionItemPropertyChanged += ModelKeys_CollectionItemPropertyChanged;
        }

        private void UnsubscribeModelKeys()
        {
            Model.Keys.CollectionItemChanged -= ModelKeys_CollectionItemChanged;
            Model.Keys.CollectionItemPropertyChanged -= ModelKeys_CollectionItemPropertyChanged;
        }

        private void UpdateLayers()
        {
            _layers.ReplaceUsingClear(Model.Layers.Select(x => new LayerViewModel(x)));
        }

        private void UpdateKeys()
        {
            _keys.ReplaceUsingClear(Model.Keys.Select(x => new KeyViewModel(x)));
        }

        private void UpdateRotationOrigin(bool alwaysIncrementVersion)
        {
            if (_keyForm.RotationOriginX.IsSet && _keyForm.RotationOriginY.IsSet &&
                (_keyForm.RotationOriginX.Value != 0 || _keyForm.RotationOriginY.Value != 0 || (_keyForm.RotationAngle.IsSet && _keyForm.RotationAngle.Value != 0)))
            {
                if (alwaysIncrementVersion || !SelectedKeysRotationOrigin.IsSet)
                {
                    SelectedKeysRotationOrigin.Version++;
                }
                SelectedKeysRotationOrigin.Value = new Point(_keyForm.RotationOriginX.Value!.Value, _keyForm.RotationOriginY.Value!.Value);
                SelectedKeysRotationOrigin.IsSet = true;
            }
            else
            {
                SelectedKeysRotationOrigin.IsSet = false;
            }
        }

        private void UpdateCanvas()
        {
            if (!_keys.Any())
            {
                CanvasOffset = new Thickness();
                CanvasSize = new Size();
                return;
            }

            var left = _keys.Min(x => x.ActualBounds.Left);
            var top = _keys.Min(x => x.ActualBounds.Top);
            var right = _keys.Max(x => x.ActualBounds.Right);
            var bottom = _keys.Max(x => x.ActualBounds.Bottom);

            CanvasOffset = new Thickness(-left, -top, left, top);
            CanvasSize = new Size(right - left, bottom - top);
        }

        private void UpdateBackground()
        {
            Background = HexColor.TryParse(Model.Background, out var color) ? color.ToColor() : DefaultBackground;
        }

        private void Tabs_CollectionItemChanged(IObservableObjectCollection<FormTabViewModel> collection, CollectionItemChangedEventArgs<FormTabViewModel> args)
        {
            var newSelectedItems = args.NewItems.Where(x => x.IsSelected).ToList();
            if (newSelectedItems.Count > 0)
            {
                var item = newSelectedItems.Last();
                _selectedTabs.ReplaceUsingMerge(_selectedTabs
                    .Except(args.OldItems)
                    .Concat(newSelectedItems.SkipLast(1))
                    .Where(x => x.Form == item.Form)
                    .Append(item));
            }
            else
            {
                _selectedTabs.RemoveRange(args.OldItems);
            }
        }

        private void Tabs_CollectionItemPropertyChanged(FormTabViewModel item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(FormTabViewModel.IsSelected):
                    if (item.IsSelected)
                    {
                        _selectedTabs.ReplaceUsingMerge(_selectedTabs.Where(x => x.Form == item.Form).Append(item));
                    }
                    break;
            }
        }

        private void SelectedTabs_CollectionItemChanged(IObservableObjectCollection<FormTabViewModel> collection, CollectionItemChangedEventArgs<FormTabViewModel> args)
        {
            foreach (var item in args.OldItems)
            {
                item.IsSelected = false;

                // TODO: Ugly
                if (item is LayerFormTabViewModel layerTab)
                {
                    foreach (var layer in _layers)
                    {
                        if (layer.Model == layerTab.Model)
                        {
                            layer.IsSelected = false;
                        }
                    }
                }
            }

            foreach (var item in args.NewItems)
            {
                item.IsSelected = true;

                // TODO: Ugly
                if (item is LayerFormTabViewModel layerTab)
                {
                    foreach (var layer in _layers)
                    {
                        if (layer.Model == layerTab.Model)
                        {
                            layer.IsSelected = true;
                        }
                    }
                }
            }

            OnSelectedTabsChanged();
        }

        private void SelectedTabs_CollectionItemPropertyChanged(FormTabViewModel item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(FormTabViewModel.IsSelected):
                    if (!item.IsSelected)
                    {
                        _selectedTabs.Remove(item);
                    }
                    break;
            }
        }

        private void OnSelectedTabsChanged()
        {
            if (_selectedTabs.Count == 0)
            {
                _selectedTabs.Add(_fileTab);
            }
            else
            {
                CurrentForm = _selectedTabs.First().Form;
            }
        }

        private void ModelLayers_CollectionItemChanged(IObservableObjectCollection<LayerModel> collection, CollectionItemChangedEventArgs<LayerModel> args)
        {
            _layers.ReplaceUsingMerge(_layers
                .Except(args.OldItems.Join(_layers, model => model, layer => layer.Model, (_, layer) => layer))
                .Concat(args.NewItems.Select(x => new LayerViewModel(x))));
        }

        private void ModelLayers_CollectionItemPropertyChanged(LayerModel item, CollectionItemPropertyChangedEventArgs args)
        {
        }

        private void Layers_CollectionItemChanged(IObservableObjectCollection<LayerViewModel> collection, CollectionItemChangedEventArgs<LayerViewModel> args)
        {
            var newSelectedItems = args.NewItems.Where(x => x.IsSelected).ToList();
            if (newSelectedItems.Count > 0)
            {
                _selectedLayers.ReplaceUsingMerge(_selectedLayers
                    .Except(args.OldItems)
                    .Concat(newSelectedItems));
            }
            else
            {
                _selectedLayers.RemoveRange(args.OldItems);
            }

            // TODO: Ugly
            if (args.OldItems.Count > 0)
            {
                var tabs = LayerTabs.Where(x => args.OldItems.Any(a => a.Model == x.Model)).ToList();
                LayerTabs.RemoveRange(tabs);
                _tabs.RemoveRange(tabs);
            }

            // TODO: Ugly
            if (args.NewItems.Count > 0)
            {
                var tabs = args.NewItems.Select(x => new LayerFormTabViewModel(x.Model, "\uE81E", _legendForm)
                {
                    DeleteCommand = new DelegateCommand(() => DeleteLayer(x.Model)),
                }).ToList();
                LayerTabs.AddRange(tabs);
                _tabs.AddRange(tabs);
            }
        }

        private void Layers_CollectionItemPropertyChanged(LayerViewModel item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(LayerViewModel.IsSelected):
                    if (item.IsSelected)
                    {
                        _selectedLayers.Merge(item);
                    }
                    break;
            }
        }

        private void SelectedLayers_CollectionItemChanged(IObservableObjectCollection<LayerViewModel> collection, CollectionItemChangedEventArgs<LayerViewModel> args)
        {
            foreach (var item in args.OldItems)
            {
                item.IsSelected = false;

                // TODO: Ugly
                foreach (var layerTab in LayerTabs)
                {
                    if (layerTab.Model == item.Model)
                    {
                        layerTab.IsSelected = false;
                    }
                }
            }

            foreach (var item in args.NewItems)
            {
                item.IsSelected = true;

                // TODO: Ugly
                foreach (var layerTab in LayerTabs)
                {
                    if (layerTab.Model == item.Model)
                    {
                        layerTab.IsSelected = true;
                    }
                }
            }

            OnSelectedLayersChanged();
        }

        private void SelectedLayers_CollectionItemPropertyChanged(LayerViewModel item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(LayerViewModel.IsSelected):
                    if (!item.IsSelected)
                    {
                        _selectedLayers.Remove(item);
                    }
                    break;
            }
        }

        private void OnSelectedLayersChanged()
        {
            UpdateLegendForm();
        }

        private void ModelKeys_CollectionItemChanged(IObservableObjectCollection<KeyModel> collection, CollectionItemChangedEventArgs<KeyModel> args)
        {
            _keys.ReplaceUsingMerge(_keys
                .Except(args.OldItems.Join(_keys, model => model, key => key.Model, (_, key) => key))
                .Concat(args.NewItems.Select(x => new KeyViewModel(x))));
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
            var newSelectedItems = args.NewItems.Where(x => x.IsSelected).ToList();
            if (newSelectedItems.Count > 0)
            {
                _selectedKeys.ReplaceUsingMerge(_selectedKeys
                    .Except(args.OldItems)
                    .Concat(newSelectedItems));
            }
            else
            {
                _selectedKeys.RemoveRange(args.OldItems);
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
                        _selectedKeys.Merge(item);
                    }
                    break;
            }
        }

        private void SelectedKeys_CollectionItemChanged(IObservableObjectCollection<KeyViewModel> collection, CollectionItemChangedEventArgs<KeyViewModel> args)
        {
            foreach (var item in args.OldItems)
            {
                item.IsSelected = false;
            }

            foreach (var item in args.NewItems)
            {
                item.IsSelected = true;
            }

            OnSelectedKeysChanged();
        }

        private void SelectedKeys_CollectionItemPropertyChanged(KeyViewModel item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyViewModel.IsSelected):
                    if (!item.IsSelected)
                    {
                        _selectedKeys.Remove(item);
                    }
                    break;
            }
        }

        private void OnSelectedKeysChanged()
        {
            UpdateKeyForm();
            UpdateLegendForm();

            DeleteSelectedKeysCommand.RaiseCanExecuteChanged();

            UpdateRotationOrigin(true);
        }

        private void UpdateKeyForm()
        {
            _keyForm.Set(_selectedKeys.Select(x => x.Model));
        }

        private void UpdateLegendForm()
        {
            var selectedModels = _layers.Where(x => x.IsSelected).Select(x => x.Model).ToHashSet();
            var selectedLayerIndices = Model.Layers
                .Select((x, i) => (isSelected: selectedModels.Contains(x), index: i))
                .Where(x => x.isSelected)
                .Select(x => x.index)
                .ToList();
            _legendForm.Set(SelectedKeys.SelectMany(x => x.Model.Legends.Where((_, i) => selectedLayerIndices.Contains(i))));
        }

        private void AddLayer()
        {
            var layerModel = new LayerModel { Name = $"Layer {Model.Layers.Count}", };

            Model.Layers.Add(layerModel);
            foreach (var keyModel in Model.Keys)
            {
                keyModel.Legends.Add(new LegendModel());
            }

            SelectLayerModel(layerModel);
        }

        private void DeleteLayer(LayerModel? model)
        {
            if (model == null)
            {
                return;
            }

            var index = Model.Layers.IndexOf(model);
            if (index < 0)
            {
                return;
            }

            Model.Layers.RemoveAt(index);
            foreach (var keyModel in Model.Keys)
            {
                if (keyModel.Legends.Count > index)
                {
                    keyModel.Legends.RemoveAt(index);
                }
            }
        }

        private void SelectLayerModel(LayerModel model)
        {
            if (_layers.FirstOrDefault(x => x.Model == model) is { } layer)
            {
                _selectedLayers.ReplaceUsingClear(new[] { layer, });
            }
        }

        private void SelectAllLayers()
        {
            _selectedTabs.ReplaceUsingClear(LayerTabs);
        }

        private void AddKey(NewKeyTemplate template)
        {
            var y = _keys.OrderByDescending(x => x.ActualBounds.Bottom).FirstOrDefault()?.ActualBounds.Bottom ?? 0;
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
            keyModel.Legends.AddRange(Model.Layers.Select(_ => new LegendModel()));

            Model.Keys.Add(keyModel);

            SelectKeyModel(keyModel);
        }

        private void SelectKeyModel(KeyModel model)
        {
            if (_keys.FirstOrDefault(x => x.Model == model) is { } key)
            {
                _selectedKeys.ReplaceUsingClear(new[] { key, });
            }
        }

        private void DeleteSelectedKeys()
        {
            Model.Keys.RemoveRange(_selectedKeys.Select(x => x.Model));
        }

        private bool CanDeleteSelectedKeys()
        {
            return _selectedKeys.Any();
        }
    }
}
