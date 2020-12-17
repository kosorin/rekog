using System.Collections.Generic;

namespace Rekog.Data
{
    public record Config : SerializationObject
    {
        public Options Options { get; set; } = default!;

        public Dictionary<string, CorpusConfig> Corpora { get; set; } = default!;

        public Dictionary<string, AlphabetConfig> Alphabets { get; set; } = default!;

        public Dictionary<string, LayoutConfig> Layouts { get; set; } = default!;

        public Dictionary<string, KeymapConfig> Keymaps { get; set; } = default!;

        protected override void FixSelf()
        {
            if (Options == null)
            {
                throw new PersistenceException("Options is not set.");
            }

            FixLocationConfigs();
            FixAlphabetConfigs();
            FixLayoutConfigs();
            FixKeymapConfigs();
        }

        private void FixLocationConfigs()
        {
            Corpora = FixMap(Corpora, x => x?.Path == null);
        }

        private void FixAlphabetConfigs()
        {
            Alphabets = FixMap(Alphabets, x => x?.Characters == null);
        }

        private void FixLayoutConfigs()
        {
            Layouts = FixMap(Layouts);
        }

        private void FixKeymapConfigs()
        {
            Keymaps = FixMap(Keymaps);
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield return Options;

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
