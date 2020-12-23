using System.Collections.Generic;
using System.Linq;

namespace Rekog.Data
{
    public record Config : SerializationObject
    {
        public Dictionary<string, CorpusConfig> Corpora { get; set; } = default!;

        public Dictionary<string, AlphabetConfig> Alphabets { get; set; } = default!;

        public Dictionary<string, LayoutConfig> Layouts { get; set; } = default!;

        public Dictionary<string, KeymapConfig> Keymaps { get; set; } = default!;

        protected override void FixSelf()
        {
            FixLocationConfigs();
            FixAlphabetConfigs();
            FixLayoutConfigs();
            FixKeymapConfigs();
        }

        private void FixLocationConfigs()
        {
            Corpora = Corpora?.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value) ?? new();
        }

        private void FixAlphabetConfigs()
        {
            Alphabets = Alphabets?.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value) ?? new();
        }

        private void FixLayoutConfigs()
        {
            Layouts = Layouts?.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value) ?? new();
        }

        private void FixKeymapConfigs()
        {
            Keymaps = Keymaps?.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value) ?? new();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            foreach (var child in Corpora.Values)
            {
                yield return child;
            }

            foreach (var child in Alphabets.Values)
            {
                yield return child;
            }

            foreach (var child in Layouts.Values)
            {
                yield return child;
            }

            foreach (var child in Keymaps.Values)
            {
                yield return child;
            }
        }
    }
}
