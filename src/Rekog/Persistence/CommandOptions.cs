using System.Collections.Generic;

namespace Rekog.Persistence
{
    public abstract record CommandOptions : SerializationObject
    {
        public string Config { get; set; } = default!;

        public string Output { get; set; } = default!;

        protected override void FixSelf()
        {
            Config ??= string.Empty;
            Output ??= string.Empty;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
