using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Persistence
{
    public record CorpusCommandOptions : CommandOptions
    {
        public bool CaseSensitive { get; set; } = default!;

        public string[] Alphabet { get; set; } = default!;

        public string[] Corpus { get; set; } = default!;

        protected override void FixSelf()
        {
            base.FixSelf();

            Alphabet = Alphabet?.Where(x => x != null).ToArray() ?? Array.Empty<string>();
            Corpus = Corpus?.Where(x => x != null).ToArray() ?? Array.Empty<string>();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            foreach (var child in base.CollectChildren())
            {
                yield return child;
            }
        }
    }
}
