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

        private KeyFormViewModel _keyForm;

        private ObservableObjectCollection<KeyViewModel> _keys = new ObservableObjectCollection<KeyViewModel>();
        private Thickness _canvasOffset;
        private Size _canvasSize;
        private Color _background = DefaultBackground;

        public BoardViewModel(BoardModel model)
            : base(model)
        {
            AddKeyCommand = new DelegateCommand(AddKey);
            DeleteSelectedKeysCommand = new DelegateCommand(DeleteSelectedKeys, CanDeleteSelectedKeys);

            UpdateAll();

            Form = new BoardFormViewModel(Model);
            _keyForm = new KeyFormViewModel();
        }

        public BoardFormViewModel Form { get; }

        public KeyFormViewModel KeyForm
        {
            get => _keyForm;
            private set => Set(ref _keyForm, value);
        }

        public DelegateCommand AddKeyCommand { get; }

        public DelegateCommand DeleteSelectedKeysCommand { get; }

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

        protected override void OnModelPropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
            base.OnModelPropertyChanging(sender, args);

            switch (args.PropertyName)
            {
                case nameof(BoardModel.Keys):
                    Model.Keys.CollectionItemChanged -= ModelKeys_CollectionItemChanged;
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
            KeyForm = new KeyFormViewModel(GetSelectedKeyModels().ToArray());
            DeleteSelectedKeysCommand.RaiseCanExecuteChanged();
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

        private void AddKey()
        {
            Model.Keys.Add(new KeyModel
            {
                Y = Keys.OrderByDescending(x => x.ActualBounds.Bottom).FirstOrDefault()?.ActualBounds.Bottom ?? 0,
            });
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
    }
}
