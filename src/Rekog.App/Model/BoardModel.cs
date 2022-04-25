using System.Linq;
using Rekog.App.Model.Kle;
using Rekog.App.ObjectModel;

namespace Rekog.App.Model
{
    public class BoardModel : ModelBase
    {
        private string _name = string.Empty;
        private string _author = string.Empty;
        private string _notes = string.Empty;
        private string _background = "#808080";
        private ObservableObjectCollection<LayerModel> _layers = new ObservableObjectCollection<LayerModel>();
        private ObservableObjectCollection<KeyModel> _keys = new ObservableObjectCollection<KeyModel>();

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

        public ObservableObjectCollection<LayerModel> Layers
        {
            get => _layers;
            set => Set(ref _layers, value);
        }

        public ObservableObjectCollection<KeyModel> Keys
        {
            get => _keys;
            set => Set(ref _keys, value);
        }

        public static BoardModel FromKle(KleBoard kleBoard)
        {
            return new BoardModel
            {
                Name = kleBoard.Name,
                Author = kleBoard.Author,
                Notes = kleBoard.Notes,
                Background = kleBoard.Background,
                Layers = new ObservableObjectCollection<LayerModel>(new[]
                {
                    new LayerModel { Name = "Top Left", },
                    new LayerModel { Name = "Top Center", },
                    new LayerModel { Name = "Top Right", },
                    new LayerModel { Name = "Middle Left", },
                    new LayerModel { Name = "Middle Center", },
                    new LayerModel { Name = "Middle Right", },
                    new LayerModel { Name = "Bottom Left", },
                    new LayerModel { Name = "Bottom Center", },
                    new LayerModel { Name = "Bottom Right", },
                    new LayerModel { Name = "Front Left", },
                    new LayerModel { Name = "Front Center", },
                    new LayerModel { Name = "Front Right", },
                }),
                Keys = new ObservableObjectCollection<KeyModel>(kleBoard.Keys.Select(KeyModel.FromKle)),
            };
        }
    }
}
