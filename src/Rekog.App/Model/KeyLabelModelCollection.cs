using System.Collections.Generic;
using Rekog.App.ObjectModel;

namespace Rekog.App.Model
{
    public class KeyLabelModelCollection : ObservableObjectCollection<KeyLabelModel>
    {
        public KeyLabelModelCollection()
        {
        }

        public KeyLabelModelCollection(IEnumerable<KeyLabelModel> collection) : base(collection)
        {
        }
    }
}
