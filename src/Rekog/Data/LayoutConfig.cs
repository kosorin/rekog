using System.Collections.Generic;

namespace Rekog.Data
{
    public record LayoutConfig : SerializationObject
    {
        public string Fingers { get; set; } = default!;

        protected override void FixSelf()
        {
            Fingers ??= string.Empty;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
