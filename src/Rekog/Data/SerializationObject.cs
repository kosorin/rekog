using System.Collections.Generic;

namespace Rekog.Data
{
    public abstract record SerializationObject
    {
        public void FixAll()
        {
            FixSelf();

            foreach (var child in CollectChildren())
            {
                child.FixAll();
            }
        }

        protected abstract void FixSelf();

        protected abstract IEnumerable<SerializationObject> CollectChildren();
    }
}
