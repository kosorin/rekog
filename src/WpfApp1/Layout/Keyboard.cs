using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace WpfApp1.Layout
{
    public class Keyboard : ObservableObject
    {
        private ObservableCollection<Key> _keys = new ObservableCollection<Key>();

        public ObservableCollection<Key> Keys
        {
            get => _keys;
            set => Set(ref _keys, value);
        }
    }
}
