using System.Collections.Generic;

namespace Rekog.Persistence
{
    public abstract record CommandConfig<TOptions> : DataObject
        where TOptions : CommandOptions
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TOptions Options { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void FixSelf()
        {
            if (Options == null)
            {
                throw new PersistenceException("Options is not set.");
            }
        }

        protected override IEnumerable<DataObject> CollectChildren()
        {
            yield return Options;
        }
    }
}
