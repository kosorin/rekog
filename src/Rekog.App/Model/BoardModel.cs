using Rekog.App.ObjectModel;

namespace Rekog.App.Model
{
    public class BoardModel : ModelBase
    {
        private string _name = string.Empty;
        private string _author = string.Empty;
        private string _notes = string.Empty;
        private string _background = "#EEEEEE";
        private ObservableDictionary<KeyId, KeyModel> _keys = new ObservableDictionary<KeyId, KeyModel>();
        private ObservableDictionary<LayerId, LayerModel> _layers = new ObservableDictionary<LayerId, LayerModel>();
        private ObservableDictionary<LegendId, LegendModel> _legends = new ObservableDictionary<LegendId, LegendModel>();

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Author
        {
            get => _author;
            set => Set(ref _author, value);
        }

        public string Notes
        {
            get => _notes;
            set => Set(ref _notes, value);
        }

        public string Background
        {
            get => _background;
            set => Set(ref _background, value);
        }

        public ObservableDictionary<KeyId, KeyModel> Keys
        {
            get => _keys;
            set => Set(ref _keys, value);
        }

        public ObservableDictionary<LayerId, LayerModel> Layers
        {
            get => _layers;
            set => Set(ref _layers, value);
        }

        public ObservableDictionary<LegendId, LegendModel> Legends
        {
            get => _legends;
            set => Set(ref _legends, value);
        }
    }
}
