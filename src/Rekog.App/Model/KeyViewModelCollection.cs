using System.Collections.Generic;
using Rekog.App.ObjectModel;

namespace Rekog.App.Model
{
    public class KeyModelCollection : ObservableObjectCollection<KeyModel>
    {
        public KeyModelCollection()
        {
        }

        public KeyModelCollection(IEnumerable<KeyModel> collection) : base(collection)
        {
        }
    }
}
