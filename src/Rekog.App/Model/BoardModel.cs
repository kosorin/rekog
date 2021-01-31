using Rekog.App.ObjectModel;

namespace Rekog.App.Model
{
    public class BoardModel : ModelBase
    {
        private ObservableObjectCollection<KeyModel> _keys = new();
        public ObservableObjectCollection<KeyModel> Keys
        {
            get => _keys;
            set => Set(ref _keys, value);
        }
    }
}
