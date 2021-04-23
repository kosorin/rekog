using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Data
{
    public record CorpusConfig : SerializationObject
    {
        public string? Path { get; set; }

        public string Pattern { get; set; } = default!;

        public bool Recursive { get; set; } = default!;

        public string Encoding { get; set; } = default!;

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        protected override void FixSelf()
        {
            Pattern ??= string.Empty;
            Encoding ??= string.Empty;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
