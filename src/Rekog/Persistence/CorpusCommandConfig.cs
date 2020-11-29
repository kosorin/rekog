using System.Collections.Generic;
using System.Linq;

namespace Rekog.Persistence
{
    public record CorpusCommandConfig : CommandConfig<CorpusCommandOptions>
    {
        public Dictionary<string, AlphabetConfig> Alphabets { get; set; } = default!;

        public Dictionary<string, LocationConfig> Locations { get; set; } = default!;

        protected override void FixSelf()
        {
            base.FixSelf();

            FixAlphabetConfigs();
            FixLocationConfigs();
        }

        private void FixAlphabetConfigs()
        {
            Alphabets ??= new();

            var toRemove = Alphabets.Where(x => x.Value?.Characters == null).Select(x => x.Key).ToList();
            foreach (var key in toRemove)
            {
                Alphabets.Remove(key);
            }
        }

        private void FixLocationConfigs()
        {
            Locations ??= new();

            var toRemove = Locations.Where(x => x.Value?.Path == null).Select(x => x.Key).ToList();
            foreach (var key in toRemove)
            {
                Locations.Remove(key);
            }
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            foreach (var child in base.CollectChildren())
            {
                yield return child;
            }

            foreach (var child in Alphabets.Values)
            {
                yield return child;
            }

            foreach (var child in Locations.Values)
            {
                yield return child;
            }
        }
    }
}
