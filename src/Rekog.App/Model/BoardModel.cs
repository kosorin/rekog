using System.Collections.ObjectModel;
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
        private ObservableCollection<string> _layers = new ObservableCollection<string>();
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

        public ObservableCollection<string> Layers
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
                Layers = new ObservableCollection<string>(new[]
                {
                    "Top Left",
                    "Top Center",
                    "Top Right",
                    "Middle Left",
                    "Middle Center",
                    "Middle Right",
                    "Bottom Left",
                    "Bottom Center",
                    "Bottom Right",
                    "Front Left",
                    "Front Center",
                    "Front Right",
                }),
                Keys = new ObservableObjectCollection<KeyModel>(kleBoard.Keys.Select(KeyModel.FromKle)),
            };
        }
    }
}
