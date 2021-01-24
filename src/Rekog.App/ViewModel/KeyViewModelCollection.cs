using Rekog.App.ObjectModel;
using System.Collections.Generic;

namespace Rekog.App.ViewModel
{
    public class KeyViewModelCollection : ObservableObjectCollection<KeyViewModel>
    {
        public KeyViewModelCollection()
        {
        }

        public KeyViewModelCollection(IEnumerable<KeyViewModel> collection) : base(collection)
        {
        }
    }
}
