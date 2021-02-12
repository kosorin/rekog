using Rekog.App.Model;
using Rekog.App.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Rekog.App.ViewModel
{
    public class BoardViewModel : ViewModelBase<BoardModel>
    {
        public BoardViewModel(BoardModel model)
            : base(model)
        {
            UpdateKeys();
            Model.Keys.CollectionItemChanged += ModelKeys_CollectionItemChanged;
        }

        private ObservableObjectCollection<KeyViewModel> _keys = new();
        public ObservableObjectCollection<KeyViewModel> Keys
        {
            get => _keys;
            private set
            {
                if (SetCollection(ref _keys, value, Keys_CollectionItemChanged, Keys_CollectionItemPropertyChanged))
                {
                    UpdateCanvas();
                }
            }
        }

        private ObservableObjectCollection<KeyViewModel> _selectedKeys = new();
        public ObservableObjectCollection<KeyViewModel> SelectedKeys
        {
            get => _selectedKeys;
            private set => Set(ref _selectedKeys, value);
        }

        private Thickness _canvasOffset;
        public Thickness CanvasOffset
        {
            get => _canvasOffset;
            private set => Set(ref _canvasOffset, value);
        }

        private Size _canvasSize;
        public Size CanvasSize
        {
            get => _canvasSize;
            private set => Set(ref _canvasSize, value);
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
                Model.Keys.CollectionItemChanged += ModelKeys_CollectionItemChanged;
                break;
            }
        }

        private void UpdateKeys()
        {
            Keys = new(Model.Keys.Select(x => new KeyViewModel(x)));
        }

        private void ModelKeys_CollectionItemChanged(IObservableObjectCollection collection, CollectionItemChangedEventArgs args)
        {
            foreach (KeyModel oldKeyModel in args.OldItems)
            {
                for (var i = Keys.Count - 1; i >= 0; i--)
                {
                    if (Keys[i].Model == oldKeyModel)
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
                if (item is KeyViewModel key)
                {
                    if (key.IsSelected)
                    {
                        if (!SelectedKeys.Contains(key))
                        {
                            SelectedKeys.Add(key);
                        }
                    }
                    else
                    {
                        SelectedKeys.Remove(key);
                    }
                }
                break;
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
    }
}
