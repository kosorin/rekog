using System.Collections.Generic;
using System.Linq;

namespace Rekog.Persistence
{
    public record CorpusCommandConfig : CommandConfig<CorpusCommandOptions>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Dictionary<string, AlphabetConfig> Alphabets { get; set; }

        public Dictionary<string, LocationConfig> Locations { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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

        protected override IEnumerable<DataObject> CollectChildren()
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
