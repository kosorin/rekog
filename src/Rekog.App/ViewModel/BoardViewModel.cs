using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Rekog.App.Extensions;
using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ViewModel.Forms;

namespace Rekog.App.ViewModel
{
    public class BoardViewModel : ViewModelBase<BoardModel>
    {
        private static readonly Color DefaultBackground = Colors.White;

        private int? _selectingKeysCounter;

        private ObservableObjectCollection<KeyViewModel> _keys = new ObservableObjectCollection<KeyViewModel>();
        private Thickness _canvasOffset;
        private Size _canvasSize;
        private Color _background = DefaultBackground;
        private bool _showRotationOrigin;

        public BoardViewModel(BoardModel model)
            : base(model)
        {
            AddKeyCommand = new DelegateCommand<NewKeyTemplate>(AddKey);
            DeleteSelectedKeysCommand = new DelegateCommand(DeleteSelectedKeys, CanDeleteSelectedKeys);
            SelectingKeysCommand = new DelegateCommand<bool>(SelectingKeys);

            UpdateAll();

            Form.Set(Model);
        }

        public BoardFormViewModel Form { get; } = new BoardFormViewModel();

        public KeyFormViewModel KeyForm { get; } = new KeyFormViewModel();

        public LegendFormViewModel LegendForm { get; } = new LegendFormViewModel();

        public DelegateCommand<NewKeyTemplate> AddKeyCommand { get; }

        public DelegateCommand DeleteSelectedKeysCommand { get; }

        public DelegateCommand<bool> SelectingKeysCommand { get; }

        public ObservableObjectCollection<KeyViewModel> Keys
        {
            get => _keys;
            private set
            {
                if (SetCollection(ref _keys, value, Keys_CollectionItemChanged, Keys_CollectionItemPropertyChanged))
                {
                    UpdateSelectedKeys();
                    UpdateCanvas();
                }
            }
        }

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
            set => Set(ref _background, value);
        }

        public bool ShowRotationOrigin
        {
            get => _showRotationOrigin;
            private set => Set(ref _showRotationOrigin, value);
        }

        protected override void OnModelPropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
            base.OnModelPropertyChanging(sender, args);

            switch (args.PropertyName)
            {
                case nameof(BoardModel.Keys):
                    Model.Keys.CollectionItemChanged -= ModelKeys_CollectionItemChanged;
                    Model.Keys.CollectionItemPropertyChanged -= ModelKeys_CollectionItemPropertyChanged;
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
                    break;
                case nameof(BoardModel.Background):
                    UpdateBackground();
                    break;
            }
        }

        private IEnumerable<KeyModel> GetSelectedKeyModels()
        {
            return Keys.Where(x => x.IsSelected).Select(x => x.Model);
        }

        private void UpdateAll()
        {
            UpdateKeys();
            UpdateBackground();
        }

        private void UpdateKeys()
        {
            Keys = new ObservableObjectCollection<KeyViewModel>(Model.Keys.Where(x => !x.IsDecal).Select(x => new KeyViewModel(x)));
            Model.Keys.CollectionItemChanged += ModelKeys_CollectionItemChanged;
            Model.Keys.CollectionItemPropertyChanged += ModelKeys_CollectionItemPropertyChanged;
        }

        private void UpdateBackground()
        {
            try
            {
                Background = Model.Background.ToColor();
            }
            catch
            {
                Background = DefaultBackground;
            }
        }

        private void ModelKeys_CollectionItemChanged(IObservableObjectCollection collection, CollectionItemChangedEventArgs args)
        {
            foreach (KeyModel oldKeyModel in args.OldItems)
            {
                for (var i = Keys.Count - 1; i >= 0; i--)
                {
                    var key = Keys[i];
                    if (key.Model == oldKeyModel)
                    {
                        Keys.RemoveAt(i);
                    }
                }
            }

            foreach (KeyModel newKeyModel in args.NewItems)
            {
                Keys.Add(new KeyViewModel(newKeyModel));
            }
        }

        private void ModelKeys_CollectionItemPropertyChanged(object item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyModel.RotationAngle):
                case nameof(KeyModel.RotationOriginX):
                case nameof(KeyModel.RotationOriginY):
                    UpdateRotationOrigin();
                    break;
            }
        }

        private void Keys_CollectionItemChanged(ICollection collection, CollectionItemChangedEventArgs args)
        {
            UpdateSelectedKeys();
            UpdateCanvas();
        }

        private void Keys_CollectionItemPropertyChanged(object item, CollectionItemPropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(KeyViewModel.ActualBounds):
                    UpdateCanvas();
                    break;
                case nameof(KeyViewModel.IsSelected):
                    UpdateSelectedKeys();
                    break;
            }
        }

        private void UpdateSelectedKeys()
        {
            if (_selectingKeysCounter.HasValue)
            {
                _selectingKeysCounter++;
                return;
            }

            KeyForm.Set(GetSelectedKeyModels());
            LegendForm.Set(GetSelectedKeyModels().Select(x => x.Legends.FirstOrDefault()).NotNull());

            DeleteSelectedKeysCommand.RaiseCanExecuteChanged();

            UpdateRotationOrigin();
        }

        private void UpdateRotationOrigin()
        {
            if (KeyForm.RotationOriginX.IsSet && KeyForm.RotationOriginY.IsSet)
            {
                ShowRotationOrigin = (KeyForm.RotationAngle.IsSet && KeyForm.RotationAngle.Value != 0)
                    || KeyForm.RotationOriginX.Value != 0
                    || KeyForm.RotationOriginY.Value != 0;
            }
            else
            {
                ShowRotationOrigin = false;
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

        private void SelectKey(KeyModel model)
        {
            SelectingKeys(true);
            try
            {
                foreach (var key in Keys)
                {
                    key.IsSelected = key.Model == model;
                }
            }
            finally
            {
                SelectingKeys(false);
            }
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

            SelectKey(keyModel);
        }

        private void DeleteSelectedKeys()
        {
            foreach (var model in GetSelectedKeyModels().ToList())
            {
                Model.Keys.Remove(model);
            }
        }

        private bool CanDeleteSelectedKeys()
        {
            return GetSelectedKeyModels().Any();
        }

        private void SelectingKeys(bool selectingKeys)
        {
            if (selectingKeys)
            {
                _selectingKeysCounter ??= 0;
            }
            else
            {
                var counter = _selectingKeysCounter ?? 0;

                _selectingKeysCounter = null;

                if (counter > 0)
                {
                    UpdateSelectedKeys();
                }
            }
        }
    }
}
