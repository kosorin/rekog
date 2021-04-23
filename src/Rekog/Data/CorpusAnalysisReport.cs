using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Data
{
    public record CorpusAnalysisReport : SerializationObject
    {
        public Dictionary<string, ulong> Unigrams { get; set; } = default!;

        public Dictionary<string, ulong> Bigrams { get; set; } = default!;

        public Dictionary<string, ulong> Trigrams { get; set; } = default!;

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        protected override void FixSelf()
        {
            Unigrams ??= new Dictionary<string, ulong>();
            Bigrams ??= new Dictionary<string, ulong>();
            Trigrams ??= new Dictionary<string, ulong>();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
