using System.Collections.Generic;

namespace Rekog.Persistence
{
    public abstract record DataObject
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

        protected abstract IEnumerable<DataObject> CollectChildren();
    }
}
