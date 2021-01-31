using Rekog.App.ObjectModel;
using System.Collections.Generic;

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
