using Rekog.App.ObjectModel;
using System.Collections;
using System.Collections.Generic;
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
                    oldValue.CollectionItemChanged -= NewValue_CollectionItemChanged;
                    oldValue.ItemPropertyChanged -= OldKeys_ItemPropertyChanged;
                }

                if (Set(ref _keys, value, nameof(Keys)))
                {
                    if (Keys is KeyViewModelCollection newValue)
                    {
                        newValue.CollectionItemChanged -= NewValue_CollectionItemChanged;
                        newValue.CollectionItemChanged += NewValue_CollectionItemChanged;
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

        private void NewValue_CollectionItemChanged(ICollection? collection, CollectionItemChangedEventArgs args)
        {
            if (args.NewItems is ICollection<KeyViewModel> keys)
            {
                UpdateKeys(keys);
            }
        }

        private void OldKeys_ItemPropertyChanged(object? item, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
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
