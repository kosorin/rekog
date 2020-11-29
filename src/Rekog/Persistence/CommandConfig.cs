using System.Collections.Generic;

namespace Rekog.Persistence
{
    public abstract record CommandConfig<TOptions> : SerializationObject
        where TOptions : CommandOptions
    {
        public TOptions Options { get; set; } = default!;

        protected override void FixSelf()
        {
            if (Options == null)
            {
                throw new PersistenceException("Options is not set.");
            }
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield return Options;
        }
    }
}
