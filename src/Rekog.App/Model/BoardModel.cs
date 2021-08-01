using System.Linq;
using PropertyTools.DataAnnotations;
using Rekog.App.Converters;
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
        private ObservableObjectCollection<KeyModel> _keys = new ObservableObjectCollection<KeyModel>();

        [Category("Board")]
        [SortIndex(0)]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        [Category("Board")]
        [SortIndex(1)]
        public string Author
        {
            get => _author;
            set => Set(ref _author, value);
        }

        [Category("Board")]
        [SortIndex(2)]
        public string Notes
        {
            get => _notes;
            set => Set(ref _notes, value);
        }

        [Category("Board")]
        [Converter(typeof(StringToColorConverter))]
        [SortIndex(3)]
        public string Background
        {
            get => _background;
            set => Set(ref _background, value);
        }

        [Browsable(false)]
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
                Keys = new ObservableObjectCollection<KeyModel>(kleBoard.Keys.Select(KeyModel.FromKle)),
            };
        }
    }
}
