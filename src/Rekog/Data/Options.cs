using System.Collections.Generic;

namespace Rekog.Data
{
    public record Options : SerializationObject
    {
        public string? DataRoot { get; set; }

        public string Corpus { get; set; } = default!;

        public string Alphabet { get; set; } = default!;

        public string Layout { get; set; } = default!;

        public string Keymap { get; set; } = default!;

        protected override void FixSelf()
        {
            Corpus ??= string.Empty;
            Alphabet ??= string.Empty;
            Layout ??= string.Empty;
            Keymap ??= string.Empty;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
