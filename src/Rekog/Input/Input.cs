using System;
using System.Collections.Generic;

namespace Rekog.Input
{
    public abstract class Input
    {
        public void Fix()
        {
            try
            {
                FixSelf();

                foreach (var input in CollectChildren())
                {
                    input.Fix();
                }
            }
            catch (Exception e) when (e is not InputException)
            {
                throw new InputException("Unknown input exception", e);
            }
        }

        protected abstract void FixSelf();

        protected abstract IEnumerable<Input> CollectChildren();
    }
}
