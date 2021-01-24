using Rekog.App.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Rekog.App.ViewModel
{
    public class BoardViewModel : ObservableObject
    {
        private double _scale = KeyViewModel.DefaultScale;
        public double Scale
        {
            get => _scale;
            set
            {
                if (value < KeyViewModel.MinimumScale)
                {
                    value = KeyViewModel.MinimumScale;
                }

                if (Set(ref _scale, value, nameof(Scale)))
                {
                    UpdateKeys();
                }
            }
        }

        private KeyViewModelCollection _keys = new KeyViewModelCollection();
        public KeyViewModelCollection Keys
        {
            get => _keys;
            set
            {
                if (Keys is KeyViewModelCollection oldValue)
                {
                    oldValue.CollectionChanged -= Keys_CollectionChanged;
                    oldValue.ItemPropertyChanged -= OldKeys_ItemPropertyChanged;
                }

                if (Set(ref _keys, value, nameof(Keys)))
                {
                    if (Keys is KeyViewModelCollection newValue)
                    {
                        newValue.CollectionChanged -= Keys_CollectionChanged;
                        newValue.CollectionChanged += Keys_CollectionChanged;
                        newValue.ItemPropertyChanged -= OldKeys_ItemPropertyChanged;
                        newValue.ItemPropertyChanged += OldKeys_ItemPropertyChanged;
                    }

                    UpdateKeys();
                    UpdateBoard();
                }
            }
        }

        private Thickness _canvasMargin;
        public Thickness CanvasMargin
        {
            get => _canvasMargin;
            private set => Set(ref _canvasMargin, value, nameof(CanvasMargin));
        }

        private Size _canvasSize;
        public Size CanvasSize
        {
            get => _canvasSize;
            private set => Set(ref _canvasSize, value, nameof(CanvasSize));
        }

        private void Keys_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Replace:
                if (e.NewItems is ICollection<KeyViewModel> keys)
                {
                    UpdateKeys(keys);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                UpdateKeys();
                break;
            }
        }

        private void OldKeys_ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
            case nameof(KeyViewModel.RotatedBounds):
                UpdateBoard();
                break;
            }
        }

        private void UpdateBoard()
        {
            var left = Keys.Min(x => x.RotatedBounds.Left);
            var top = Keys.Min(x => x.RotatedBounds.Top);
            var right = Keys.Max(x => x.RotatedBounds.Right);
            var bottom = Keys.Max(x => x.RotatedBounds.Bottom);
            CanvasMargin = new Thickness(-left, -top, left, top);
            CanvasSize = new Size(right - left, bottom - top);
        }

        private void UpdateKeys()
        {
            UpdateKeys(Keys);
        }

        private void UpdateKeys(ICollection<KeyViewModel> keys)
        {
            foreach (var key in keys)
            {
                key.Scale = Scale;
            }
        }
    }
}
