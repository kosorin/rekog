using Rekog.App.Model;

namespace Rekog.App.ViewModel
{
    public class LayerViewModel : ViewModelBase<LayerModel>
    {
        private bool _isSelected;

        public LayerViewModel(LayerModel model)
            : base(model)
        {
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
    }
}
