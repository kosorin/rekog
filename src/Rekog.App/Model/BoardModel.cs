using PropertyTools.DataAnnotations;
using Rekog.App.Converters;
using Rekog.App.ObjectModel;

namespace Rekog.App.Model
{
    public class BoardModel : ModelBase
    {
        private ObservableObjectCollection<KeyModel> _keys = new ObservableObjectCollection<KeyModel>();
        private string _background = "#808080";

        [Browsable(false)]
        public ObservableObjectCollection<KeyModel> Keys
        {
            get => _keys;
            set => Set(ref _keys, value);
        }

        [Category("Board")]
        [Converter(typeof(StringToColorConverter))]
        [SortIndex(0)]
        public string Background
        {
            get => _background;
            set => Set(ref _background, value);
        }
    }
}
