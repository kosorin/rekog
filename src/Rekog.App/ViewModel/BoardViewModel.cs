using Rekog.App.Model;
using Rekog.App.ObjectModel;
using System.Collections;
using System.Linq;
using System.Windows;

namespace Rekog.App.ViewModel
{
    public class BoardViewModel : ViewModelBase<BoardModel>
    {
        public BoardViewModel()
        {
        }

        public BoardViewModel(BoardModel? model)
            : base(model)
        {
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

        protected override void OnModelChanged(BoardModel? oldModel, BoardModel? newModel)
        {
            base.OnModelChanged(oldModel, newModel);

            if (newModel?.Keys != null)
            {
                Keys = new(newModel.Keys.Select(x => new KeyViewModel(x)));
            }
            else
            {
                Keys = new();
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
            case nameof(KeyViewModel.RotatedBounds):
                UpdateCanvas();
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

            var left = Keys.Min(x => x.RotatedBounds.Left);
            var top = Keys.Min(x => x.RotatedBounds.Top);
            var right = Keys.Max(x => x.RotatedBounds.Right);
            var bottom = Keys.Max(x => x.RotatedBounds.Bottom);

            CanvasOffset = new Thickness(-left, -top, left, top);
            CanvasSize = new Size(right - left, bottom - top);
        }
    }
}
