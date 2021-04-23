using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Data
{
    public record AlphabetConfig : SerializationObject
    {
        public string Characters { get; set; } = default!;

        public bool IncludeWhitespace { get; set; } = default!;

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
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
