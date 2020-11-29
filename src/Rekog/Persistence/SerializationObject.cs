using System.Collections.Generic;

namespace Rekog.Persistence
{
    public abstract record SerializationObject
    {
        public void Fix()
        {
            FixSelf();

            foreach (var child in CollectChildren())
            {
                child.Fix();
            }
        }

        protected abstract void FixSelf();

        protected abstract IEnumerable<SerializationObject> CollectChildren();
    }
}
