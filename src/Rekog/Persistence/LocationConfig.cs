using Rekog.IO;
using System.Collections.Generic;

namespace Rekog.Persistence
{
    public record LocationConfig : SerializationObject
    {
        public string Path { get; set; } = default!;

        public string Pattern { get; set; } = default!;

        public bool Recursive { get; set; } = default!;

        public string Encoding { get; set; } = default!;

        protected override void FixSelf()
        {
            Path ??= string.Empty;
            Pattern ??= PathHelper.DefaultSearchPattern;
            Encoding ??= System.Text.Encoding.UTF8.WebName;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
