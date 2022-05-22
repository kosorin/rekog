using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Koda.ColorTools;
using Koda.ColorTools.Wpf;
using Rekog.App.Forms;
using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.Reflection;
using Rekog.App.Undo;
using Rekog.App.Undo.Actions;
using Rekog.App.Undo.Batches;
using Rekog.App.ViewModel.Forms;

namespace Rekog.App.ViewModel
{
    public class BoardViewModel : ViewModelBase<BoardModel>
    {
        private static readonly Color DefaultBackground = Colors.White;
        
        private static readonly ChangePropertyUndoBatchBuilder KeyPositionUndoBatchBuilder = new ChangePropertyUndoBatchBuilder(new NamedChangePropertyGroupKey(nameof(KeyModel) + "_" + nameof(KeyModel.Position), new[]
        {
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.X)),
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.Y)),
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.RotationOriginX)),
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.RotationOriginY)),
        }));

        private static readonly ChangePropertyUndoBatchBuilder KeyRotationOriginUndoBatchBuilder = new ChangePropertyUndoBatchBuilder(new NamedChangePropertyGroupKey(nameof(KeyModel) + "_" + nameof(KeyModel.RotationOrigin), new[]
        {
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.X)),
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.Y)),
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.RotationOriginX)),
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.RotationOriginY)),
        }));

        private static readonly ChangePropertyUndoBatchBuilder KeyRotationAngleUndoBatchBuilder = new ChangePropertyUndoBatchBuilder(new NamedChangePropertyGroupKey(nameof(KeyModel) + "_" + nameof(KeyModel.RotationAngle), new[]
        {
            ReflectionCache.GetPropertyInfo<KeyModel>(nameof(KeyModel.RotationAngle)),
        }));

        private readonly PropertyObserver _modelObserver = new PropertyObserver();

        // Each item is in the collection only once.
        private readonly ObservableList<FormTab> _tabs = new ObservableList<FormTab>();
        private readonly ObservableDictionary<KeyId, KeyViewModel> _keys = new ObservableDictionary<KeyId, KeyViewModel>();
        private readonly ObservableDictionary<LayerId, LayerViewModel> _layers = new ObservableDictionary<LayerId, LayerViewModel>();

        // The selected collections are subsets of the respective collections. 
        // IsSelected properties are for caching, however can be used to select items.
        private readonly ObservableList<FormTab> _selectedTabs = new ObservableList<FormTab>();
        private readonly ObservableDictionary<KeyId, KeyViewModel> _selectedKeys = new ObservableDictionary<KeyId, KeyViewModel>();
        private readonly ObservableDictionary<LayerId, LayerViewModel> _selectedLayers = new ObservableDictionary<LayerId, LayerViewModel>();

        private Form? _currentForm;
        private readonly BoardFileForm _boardFileForm;
        private readonly BoardPropertiesForm _boardPropertiesForm;
        private readonly BoardForm _boardForm;
        private readonly KeyForm _keyForm;
        private readonly LayerForm _layerForm;

        private Rect _actualBounds;
        private Rect? _selectedKeysActualBounds;
        private Color _background = DefaultBackground;

        public BoardViewModel(BoardModel model)
            : base(model)
        {
            model.UndoActionExecuted += OnModelUndoActionExecuted;

            AddKeyCommand = new DelegateCommand<NewKeyTemplate>(AddKey);
            DeleteSelectedKeysCommand = new DelegateCommand(DeleteSelectedKeys, CanDeleteSelectedKeys);
            SelectAllKeysCommand = new DelegateCommand(SelectAllKeys, CanSelectAllKeys);
            UnselectAllKeysCommand = new DelegateCommand(UnselectAllKeys, CanUnselectAllKeys);
            ChangeSelectedKeysPositionCommand = new DelegateCommand<Point>(ChangeSelectedKeysPosition, CanChangeSelectedKeysPosition);
            ChangeSelectedKeysRotationOriginCommand = new DelegateCommand<Point>(ChangeSelectedKeysRotationOrigin, CanChangeSelectedKeysRotationOrigin);
            ChangeSelectedKeysRotationAngleCommand = new DelegateCommand<double>(ChangeSelectedKeysRotationAngle, CanChangeSelectedKeysRotationAngle);
            AddLayerCommand = new DelegateCommand(AddLayer);
            SelectAllLayersCommand = new DelegateCommand(SelectAllLayers);

            SubscribeModelKeys();
            SubscribeModelLayers();
            SubscribeModelLegends();

            _tabs.ListChanged += OnTabsListChanged;
            _tabs.ItemPropertyChanged += OnTabsItemPropertyChanged;
            _keys.DictionaryChanged += OnKeysDictionaryChanged;
            _keys.EntryPropertyChanged += OnKeysEntryPropertyChanged;
            _modelObserver.Subscribe(_keys, "", null);
            _layers.DictionaryChanged += OnLayersDictionaryChanged;
            _layers.EntryPropertyChanged += OnLayersEntryPropertyChanged;
            _selectedTabs.ListChanged += OnSelectedTabsListChanged;
            _selectedTabs.ItemPropertyChanged += OnSelectedTabsItemPropertyChanged;
            _selectedKeys.DictionaryChanged += OnSelectedKeysDictionaryChanged;
            _selectedKeys.EntryPropertyChanged += OnSelectedKeysEntryPropertyChanged;
            _selectedLayers.DictionaryChanged += OnSelectedLayersDictionaryChanged;
            _selectedLayers.EntryPropertyChanged += OnSelectedLayersEntryPropertyChanged;

            _boardFileForm = new BoardFileForm(UndoContext);
            _boardPropertiesForm = new BoardPropertiesForm(UndoContext);
            _boardForm = new BoardForm(UndoContext);
            _keyForm = new KeyForm(UndoContext);
            _layerForm = new LayerForm(UndoContext);

            Keys = _keys.Values;
            SelectedKeys = _selectedKeys;
            SelectedKeysRotationOrigin = new RotationOriginViewModel(_keyForm);

            _boardFileForm.SetModel(Model);
            _boardPropertiesForm.SetModel(Model);
            _boardForm.SetModel(Model);

            _tabs.AddRange(new[]
            {
                FileTab = new FormTab("File", "\uE130", _boardFileForm) { IsSelected = true, },
                PropertiesTab = new FormTab("Properties", "\uE946", _boardPropertiesForm),
                BoardTab = new FormTab("Board", "\uE809", _boardForm),
                KeyTab = new FormTab("Key", "\uF158", _keyForm),
            });
            StaticTabs = new ObservableCollection<FormTab>(_tabs);
            LayerTabs = new ObservableDictionary<LayerId, LayerFormTab>();
            LayerTabDropHandler = new LayerFormTabDropHandler(Model, UndoContext);

            UpdateKeys();
            UpdateLayers();
            UpdateLegends();
            UpdateBackground();
        }

        public UndoContext UndoContext { get; } = new UndoContext();

        public DelegateCommand<NewKeyTemplate> AddKeyCommand { get; }

        public DelegateCommand DeleteSelectedKeysCommand { get; }

        public DelegateCommand SelectAllKeysCommand { get; }

        public DelegateCommand UnselectAllKeysCommand { get; }

        public DelegateCommand<Point> ChangeSelectedKeysPositionCommand { get; }

        public DelegateCommand<Point> ChangeSelectedKeysRotationOriginCommand { get; }

        public DelegateCommand<double> ChangeSelectedKeysRotationAngleCommand { get; }

        public DelegateCommand AddLayerCommand { get; }

        public DelegateCommand SelectAllLayersCommand { get; }

        public Form? CurrentForm
        {
            get => _currentForm;
            private set => Set(ref _currentForm, value);
        }

        public FormTab FileTab { get; }

        public FormTab PropertiesTab { get; }

        public FormTab BoardTab { get; }

        public FormTab KeyTab { get; }

        public ObservableCollection<FormTab> StaticTabs { get; }

        public ObservableDictionary<LayerId, LayerFormTab> LayerTabs { get; }

        public LayerFormTabDropHandler LayerTabDropHandler { get; }

        public ReadOnlyObservableCollection<KeyViewModel> Keys { get; }

        public ObservableDictionary<KeyId, KeyViewModel> SelectedKeys { get; }

        public RotationOriginViewModel SelectedKeysRotationOrigin { get; }

        public Rect ActualBounds
        {
            get => _actualBounds;
            private set => Set(ref _actualBounds, value);
        }

        public Rect? SelectedKeysActualBounds
        {
            get => _selectedKeysActualBounds;
            private set => Set(ref _selectedKeysActualBounds, value);
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
                case nameof(BoardModel.Layers):
                    UnsubscribeModelLayers();
                    break;
                case nameof(BoardModel.Legends):
                    UnsubscribeModelLegends();
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
                case nameof(BoardModel.Layers):
                    UpdateLayers();
                    SubscribeModelLayers();
                    break;
                case nameof(BoardModel.Legends):
                    UpdateLegends();
                    SubscribeModelLegends();
                    break;
                case nameof(BoardModel.Background):
                    UpdateBackground();
                    break;
            }
        }

        private void OnModelUndoActionExecuted(object sender, IUndoAction action)
        {
            UndoContext.PushAction(action);
        }

        private void SubscribeModelKeys()
        {
            Model.Keys.DictionaryChanged += OnModelKeysDictionaryChanged;

            foreach (var model in Model.Keys.Values)
            {
                model.UndoActionExecuted += OnModelUndoActionExecuted;
            }
        }

        private void UnsubscribeModelKeys()
        {
            Model.Keys.DictionaryChanged -= OnModelKeysDictionaryChanged;

            foreach (var model in Model.Keys.Values)
            {
                model.UndoActionExecuted -= OnModelUndoActionExecuted;
            }
        }

        private void SubscribeModelLayers()
        {
            Model.Layers.DictionaryChanged += OnModelLayersDictionaryChanged;

            foreach (var model in Model.Layers.Values)
            {
                model.UndoActionExecuted += OnModelUndoActionExecuted;
            }
        }

        private void UnsubscribeModelLayers()
        {
            Model.Layers.DictionaryChanged -= OnModelLayersDictionaryChanged;

            foreach (var model in Model.Layers.Values)
            {
                model.UndoActionExecuted -= OnModelUndoActionExecuted;
            }
        }

        private void SubscribeModelLegends()
        {
            Model.Legends.DictionaryChanged += OnModelLegendsDictionaryChanged;

            foreach (var model in Model.Legends.Values)
            {
                model.UndoActionExecuted += OnModelUndoActionExecuted;
            }
        }

        private void UnsubscribeModelLegends()
        {
            Model.Legends.DictionaryChanged -= OnModelLegendsDictionaryChanged;

            foreach (var model in Model.Legends.Values)
            {
                model.UndoActionExecuted -= OnModelUndoActionExecuted;
            }
        }

        private void UpdateKeys()
        {
            _keys.ClearOverwrite(Model.Keys.ToDictionary(x => x.Key, x => new KeyViewModel(x.Value)));
        }

        private void UpdateLayers()
        {
            _layers.ClearOverwrite(Model.Layers.ToDictionary(x => x.Key, x => new LayerViewModel(x.Value)));
        }

        private void UpdateLegends()
        {
            foreach (var (keyId, key) in _keys)
            {
                key.Legends.ClearOverwrite(Model.Layers.Keys.Select(layerId =>
                {
                    var legendId = new LegendId(keyId, layerId);
                    return new KeyValuePair<LegendId, LegendViewModel>(legendId, new LegendViewModel(Model.Legends[legendId], _layers[layerId]));
                }));
            }
        }

        private void UpdateActualBounds()
        {
            ActualBounds = GetActualBounds(_keys.Values) ?? new Rect();
        }

        private void UpdateSelectedKeysActualBounds()
        {
            SelectedKeysActualBounds = GetActualBounds(_selectedKeys.Values);
        }

        private static Rect? GetActualBounds(ICollection<KeyViewModel> keys)
        {
            if (!keys.Any())
            {
                return null;
            }

            var left = keys.Min(x => x.ActualBounds.Left);
            var top = keys.Min(x => x.ActualBounds.Top);
            var right = keys.Max(x => x.ActualBounds.Right);
            var bottom = keys.Max(x => x.ActualBounds.Bottom);

            return new Rect(new Point(left, top), new Point(right, bottom));
        }

        private void UpdateBackground()
        {
            Background = HexColor.TryParse(Model.Background, out var color) ? color.ToColor() : DefaultBackground;
        }

        private void OnTabsListChanged(ObservableList<FormTab> list, ListChangedEventArgs<FormTab> args)
        {
            var newSelectedItems = args.NewItems.Where(x => x.IsSelected).ToList();
            if (newSelectedItems.Count > 0)
            {
                var item = newSelectedItems.Last();
                _selectedTabs.MergeOverwrite(_selectedTabs
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

        private void OnTabsItemPropertyChanged(ObservableList<FormTab> list, ItemPropertyChangedEventArgs<FormTab> args)
        {
            switch (args.PropertyName)
            {
                case nameof(FormTab.IsSelected):
                    if (args.Item.IsSelected)
                    {
                        _selectedTabs.MergeOverwrite(_selectedTabs.Where(x => x.Form == args.Item.Form).Append(args.Item));
                    }
                    break;
            }
        }

        private void OnSelectedTabsListChanged(ObservableList<FormTab> list, ListChangedEventArgs<FormTab> args)
        {
            foreach (var item in args.OldItems)
            {
                item.IsSelected = false;
                if (item is LayerFormTab layerTab && _layers.TryGetValue(layerTab.Model.Id, out var layer))
                {
                    layer.IsSelected = false;
                }
            }

            foreach (var item in args.NewItems)
            {
                item.IsSelected = true;
                if (item is LayerFormTab layerTab && _layers.TryGetValue(layerTab.Model.Id, out var layer))
                {
                    layer.IsSelected = true;
                }
            }

            OnSelectedTabsChanged();
        }

        private void OnSelectedTabsItemPropertyChanged(ObservableList<FormTab> list, ItemPropertyChangedEventArgs<FormTab> args)
        {
            switch (args.PropertyName)
            {
                case nameof(FormTab.IsSelected):
                    if (!args.Item.IsSelected)
                    {
                        _selectedTabs.Remove(args.Item);
                    }
                    break;
            }
        }

        private void OnSelectedTabsChanged()
        {
            if (_selectedTabs.Count == 0)
            {
                _selectedTabs.Add(FileTab);
            }
            else
            {
                CurrentForm = _selectedTabs.First().Form;
            }
        }

        private void OnModelKeysDictionaryChanged(ObservableDictionary<KeyId, KeyModel> dictionary, DictionaryChangedEventArgs<KeyId, KeyModel> args)
        {
            foreach (var model in args.OldEntries.Values)
            {
                model.UndoActionExecuted -= OnModelUndoActionExecuted;
            }

            foreach (var model in args.NewEntries.Values)
            {
                model.UndoActionExecuted += OnModelUndoActionExecuted;
            }

            UndoContext.PushAction(new ChangeEntriesUndoAction<KeyId, KeyModel>(dictionary, args));

            _keys.Merge(args.NewEntries.ToDictionary(x => x.Key, x => new KeyViewModel(x.Value)), args.OldEntries.Keys);
        }

        private void OnKeysDictionaryChanged(ObservableDictionary<KeyId, KeyViewModel> dictionary, DictionaryChangedEventArgs<KeyId, KeyViewModel> args)
        {
            _selectedKeys.Merge(args.NewEntries.Where(x => x.Value.IsSelected), args.OldEntries.Keys);

            UpdateActualBounds();

            SelectAllKeysCommand.RaiseCanExecuteChanged();
        }

        private void OnKeysEntryPropertyChanged(ObservableDictionary<KeyId, KeyViewModel> dictionary, EntryPropertyChangedEventArgs<KeyId, KeyViewModel> args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyViewModel.ActualBounds):
                    UpdateActualBounds();
                    break;
                case nameof(KeyViewModel.IsSelected):
                    if (args.Value.IsSelected)
                    {
                        _selectedKeys.AddOrReplace(args.Key, args.Value);
                    }
                    break;
            }
        }

        private void OnSelectedKeysDictionaryChanged(ObservableDictionary<KeyId, KeyViewModel> dictionary, DictionaryChangedEventArgs<KeyId, KeyViewModel> args)
        {
            foreach (var key in args.OldEntries.Values)
            {
                key.IsSelected = false;
            }

            foreach (var key in args.NewEntries.Values)
            {
                key.IsSelected = true;
            }

            UndoContext.SealLastBatch();

            UpdateKeyForm();
            UpdateLegendForm();

            UpdateSelectedKeysActualBounds();

            UnselectAllKeysCommand.RaiseCanExecuteChanged();
            DeleteSelectedKeysCommand.RaiseCanExecuteChanged();
        }

        private void OnSelectedKeysEntryPropertyChanged(ObservableDictionary<KeyId, KeyViewModel> dictionary, EntryPropertyChangedEventArgs<KeyId, KeyViewModel> args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyViewModel.ActualBounds):
                    UpdateSelectedKeysActualBounds();
                    break;
                case nameof(KeyViewModel.IsSelected):
                    if (!args.Value.IsSelected)
                    {
                        _selectedKeys.Remove(args.Key);
                    }
                    break;
            }
        }

        private void OnModelLayersDictionaryChanged(ObservableDictionary<LayerId, LayerModel> dictionary, DictionaryChangedEventArgs<LayerId, LayerModel> args)
        {
            foreach (var model in args.OldEntries.Values)
            {
                model.UndoActionExecuted -= OnModelUndoActionExecuted;
            }

            foreach (var model in args.NewEntries.Values)
            {
                model.UndoActionExecuted += OnModelUndoActionExecuted;
            }

            UndoContext.PushAction(new ChangeEntriesUndoAction<LayerId, LayerModel>(dictionary, args));

            _layers.Merge(args.NewEntries.ToDictionary(x => x.Key, x => new LayerViewModel(x.Value)), args.OldEntries.Keys);
        }

        private void OnLayersDictionaryChanged(ObservableDictionary<LayerId, LayerViewModel> dictionary, DictionaryChangedEventArgs<LayerId, LayerViewModel> args)
        {
            _selectedLayers.Merge(args.NewEntries.Where(x => x.Value.IsSelected), args.OldEntries.Keys);

            LayerTabs.Merge(args.NewEntries.ToDictionary(x => x.Key, x => new LayerFormTab(x.Value.Model, "\uE81E", _layerForm)
            {
                DeleteCommand = new DelegateCommand(() => DeleteLayer(x.Key)),
            }), args.OldEntries.Keys);

            _tabs.MergeOverwrite(_tabs
                .Where(x => x is not LayerFormTab)
                .Concat(LayerTabs.Select(x => x.Value)));
        }

        private void OnLayersEntryPropertyChanged(ObservableDictionary<LayerId, LayerViewModel> dictionary, EntryPropertyChangedEventArgs<LayerId, LayerViewModel> args)
        {
            switch (args.PropertyName)
            {
                case nameof(LayerViewModel.IsSelected):
                    if (args.Value.IsSelected)
                    {
                        _selectedLayers.AddOrReplace(args.Key, args.Value);
                    }
                    break;
            }
        }

        private void OnSelectedLayersDictionaryChanged(ObservableDictionary<LayerId, LayerViewModel> dictionary, DictionaryChangedEventArgs<LayerId, LayerViewModel> args)
        {
            foreach (var (layerId, layer) in args.OldEntries)
            {
                layer.IsSelected = false;
                if (LayerTabs.TryGetValue(layerId, out var layerTab))
                {
                    layerTab.IsSelected = false;
                }
            }

            foreach (var (layerId, layer) in args.NewEntries)
            {
                layer.IsSelected = true;
                if (LayerTabs.TryGetValue(layerId, out var layerTab))
                {
                    layerTab.IsSelected = true;
                }
            }

            UndoContext.SealLastBatch();

            UpdateLayerForm();
            UpdateLegendForm();
        }

        private void OnSelectedLayersEntryPropertyChanged(ObservableDictionary<LayerId, LayerViewModel> dictionary, EntryPropertyChangedEventArgs<LayerId, LayerViewModel> args)
        {
            switch (args.PropertyName)
            {
                case nameof(LayerViewModel.IsSelected):
                    if (!args.Value.IsSelected)
                    {
                        _selectedLayers.Remove(args.Key);
                    }
                    break;
            }
        }

        private void OnModelLegendsDictionaryChanged(ObservableDictionary<LegendId, LegendModel> dictionary, DictionaryChangedEventArgs<LegendId, LegendModel> args)
        {
            foreach (var model in args.OldEntries.Values)
            {
                model.UndoActionExecuted -= OnModelUndoActionExecuted;
            }

            foreach (var model in args.NewEntries.Values)
            {
                model.UndoActionExecuted += OnModelUndoActionExecuted;
            }

            UndoContext.PushAction(new ChangeEntriesUndoAction<LegendId, LegendModel>(dictionary, args));

            foreach (var group in args.OldEntries.GroupBy(x => x.Key.KeyId, x => x.Key))
            {
                _keys[group.Key].Legends.RemoveRange(group);
            }

            foreach (var group in args.NewEntries.GroupBy(x => x.Key.KeyId))
            {
                _keys[group.Key].Legends.AddRange(group.ToDictionary(x => x.Key, x => new LegendViewModel(x.Value, _layers[x.Key.LayerId])));
            }
        }

        private void UpdateKeyForm()
        {
            _keyForm.SetModels(_selectedKeys.Values.Select(x => x.Model));
        }

        private void UpdateLayerForm()
        {
            if (_selectedLayers.Count == 1)
            {
                _layerForm.SetModel(_selectedLayers.Values.Single().Model);
            }
            else
            {
                _layerForm.ClearModels();
            }
        }

        private void UpdateLegendForm()
        {
            _layerForm.LegendForm.SetModels(_selectedKeys.Keys.SelectMany(_ => _selectedLayers.Keys, (keyId, layerId) => Model.Legends[new LegendId(keyId, layerId)]));
        }

        public void AddKey(NewKeyTemplate template)
        {
            var keyId = new KeyId(Model.Keys.Count > 0 ? Model.Keys.Keys.Max(x => x.Value) + 1 : 0);
            var y = _keys.Values.OrderByDescending(x => x.ActualBounds.Bottom).FirstOrDefault()?.ActualBounds.Bottom ?? 0;
            var keyModel = template switch
            {
                NewKeyTemplate.IsoEnter => new KeyModel(keyId)
                {
                    Y = y,
                    Width = 1.25,
                    Height = 2.0,
                    UseShape = true,
                    Shape = "M-.25 0 0 0 1.25 0 1.25 1 1.25 2 0 2 0 1-.25 1-.25 0z",
                    SteppedWidth = 1.25,
                    SteppedHeight = 2.0,
                },
                NewKeyTemplate.BigAssEnter => new KeyModel(keyId)
                {
                    Y = y,
                    Width = 1.5,
                    Height = 2.0,
                    UseShape = true,
                    Shape = "M0 0 1.5 0 1.5 1 1.5 2 0 2-.75 2-.75 1 0 1 0 0z",
                    SteppedWidth = 1.5,
                    SteppedHeight = 2.0,
                },
                NewKeyTemplate.SteppedCapsLock => new KeyModel(keyId)
                {
                    Y = y,
                    Width = 1.75,
                    Height = 1,
                    IsStepped = true,
                    SteppedWidth = 1.25,
                    SteppedHeight = 1,
                },
                NewKeyTemplate.CenterStepped => new KeyModel(keyId)
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
                _ => new KeyModel(keyId)
                {
                    Y = y,
                },
            };

            using (UndoContext.Batch())
            {
                Model.Keys.Add(keyId, keyModel);
                Model.Legends.AddRange(Model.Layers.Keys.ToDictionary(x => new LegendId(keyId, x), x => new LegendModel(new LegendId(keyId, x))));
            }

            SelectKey(keyId);
        }

        public void DeleteSelectedKeys()
        {
            using (UndoContext.Batch())
            {
                Model.Legends.RemoveRange(_selectedKeys.Keys.SelectMany(_ => Model.Layers.Keys, (keyId, layerId) => new LegendId(keyId, layerId)));
                Model.Keys.RemoveRange(_selectedKeys.Keys);
            }
        }

        private bool CanDeleteSelectedKeys()
        {
            return _selectedKeys.Any();
        }

        public void SelectKey(KeyId id)
        {
            if (_keys.TryGetValue(id, out var key))
            {
                _selectedKeys.ClearOverwrite(new[] { new KeyValuePair<KeyId, KeyViewModel>(id, key), });
            }
        }

        public void SelectAllKeys()
        {
            _selectedKeys.AddOrReplaceRange(_keys);
        }

        private bool CanSelectAllKeys()
        {
            return _keys.Any();
        }

        public void UnselectAllKeys()
        {
            _selectedKeys.Clear();
        }

        private bool CanUnselectAllKeys()
        {
            return _selectedKeys.Any();
        }

        private void ChangeSelectedKeysPosition(Point offset)
        {
            using (UndoContext.Batch(KeyPositionUndoBatchBuilder))
            {
                foreach (var key in _selectedKeys.Values)
                {
                    key.Model.X += offset.X;
                    key.Model.Y += offset.Y;
                    key.Model.RotationOriginX += offset.X;
                    key.Model.RotationOriginY += offset.Y;
                }
            }
        }

        private bool CanChangeSelectedKeysPosition(Point offset)
        {
            return (offset.X != 0 || offset.Y != 0) && _selectedKeys.Any();
        }

        private void ChangeSelectedKeysRotationOrigin(Point offset)
        {
            using (UndoContext.Batch(KeyRotationOriginUndoBatchBuilder))
            {
                foreach (var key in _selectedKeys.Values)
                {
                    key.Model.RotationOriginX ??= key.Model.X;
                    key.Model.RotationOriginY ??= key.Model.Y;
                    key.Model.X += offset.X;
                    key.Model.Y += offset.Y;
                }
            }
        }

        private bool CanChangeSelectedKeysRotationOrigin(Point offset)
        {
            return (offset.X != 0 || offset.Y != 0) && _selectedKeys.Any();
        }

        private void ChangeSelectedKeysRotationAngle(double change)
        {
            using (UndoContext.Batch(KeyRotationAngleUndoBatchBuilder))
            {
                foreach (var key in _selectedKeys.Values)
                {
                    key.Model.RotationAngle = (key.Model.RotationAngle + change) % 360d;
                }
            }
        }

        private bool CanChangeSelectedKeysRotationAngle(double change)
        {
            return change != 0 && _selectedKeys.Any();
        }

        public void AddLayer()
        {
            var layerId = new LayerId(Model.Layers.Count > 0 ? Model.Layers.Keys.Max(x => x.Value) + 1 : 0);
            var layerModel = new LayerModel(layerId)
            {
                Name = $"Layer {layerId.Value}",
                Order = Model.Layers.Count > 0 ? Model.Layers.Values.Max(x => x.Order) + 1 : 0,
            };
            var legendModels = Model.Keys.Values.ToDictionary(x => new LegendId(x.Id, layerId), x => new LegendModel(new LegendId(x.Id, layerId)));

            using (UndoContext.Batch())
            {
                Model.Layers.Add(layerId, layerModel);
                Model.Legends.AddRange(legendModels);
            }

            SelectLayer(layerId);
        }

        public void SelectLayer(LayerId id)
        {
            if (_layers.TryGetValue(id, out var layer))
            {
                _selectedLayers.ClearOverwrite(new[] { new KeyValuePair<LayerId, LayerViewModel>(id, layer), });
            }
        }

        public void SelectAllLayers()
        {
            _selectedTabs.ClearOverwrite(LayerTabs.Values);
        }

        public void DeleteLayer(LayerId id)
        {
            using (UndoContext.Batch())
            {
                Model.Legends.RemoveRange(Model.Keys.Keys.Select(x => new LegendId(x, id)));
                Model.Layers.Remove(id);
            }
        }
    }
}
