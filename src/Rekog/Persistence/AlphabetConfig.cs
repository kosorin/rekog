using System.Collections.Generic;

namespace Rekog.Persistence
{
    public record AlphabetConfig : SerializationObject
    {
        public string Characters { get; set; } = default!;

        public bool IncludeWhitespace { get; set; } = default!;

        protected override void FixSelf()
        {
            Characters ??= string.Empty;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
