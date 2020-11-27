using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Input.Options
{
    public record CorpusOptions : CommandOptions
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public bool CaseSensitive { get; set; }

        public string[] Alphabet { get; set; }

        public string[] Corpus { get; set; }

        public bool IncludeIgnored { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void FixSelf()
        {
            base.FixSelf();

            Alphabet = Alphabet?.Where(x => x != null).ToArray() ?? Array.Empty<string>();
            Corpus = Corpus?.Where(x => x != null).ToArray() ?? Array.Empty<string>();
        }

        protected override IEnumerable<Input> CollectChildren()
        {
            foreach (var input in base.CollectChildren())
            {
                yield return input;
            }

            yield break;
        }
    }
}
