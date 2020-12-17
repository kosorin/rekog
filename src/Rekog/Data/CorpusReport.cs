using System.Collections.Generic;

namespace Rekog.Data
{
    public record CorpusReport : SerializationObject
    {
        public Dictionary<string, ulong> Unigrams { get; set; } = default!;

        public Dictionary<string, ulong> Bigrams { get; set; } = default!;

        public Dictionary<string, ulong> Trigrams { get; set; } = default!;

        protected override void FixSelf()
        {
            Unigrams ??= new();
            Bigrams ??= new();
            Trigrams ??= new();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
