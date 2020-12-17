using System.Collections.Generic;

namespace Rekog.Data
{
    public record CorpusConfig : SerializationObject
    {
        public string Path { get; set; } = default!;

        public string Pattern { get; set; } = default!;

        public bool Recursive { get; set; } = default!;

        public string Encoding { get; set; } = default!;

        protected override void FixSelf()
        {
            Path ??= string.Empty;
            Pattern ??= string.Empty;
            Encoding ??= string.Empty;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
