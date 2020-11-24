using System;
using System.Collections.Generic;

namespace Rekog.Input
{
    public abstract class Input
    {
        public void Fix()
        {
            FixSelf();

            foreach (var input in CollectChildren())
            {
                input.Fix();
            }
        }

        protected abstract void FixSelf();

        protected abstract IEnumerable<Input> CollectChildren();
    }
}
