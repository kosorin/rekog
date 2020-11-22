using Rekog.Input.Options;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Input.Configurations
{
    public class CorpusConfig : CommandConfig<CorpusOptions>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Dictionary<string, AlphabetConfig> AlphabetConfigs { get; set; }

        public Dictionary<string, PathConfig> PathConfigs { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void FixSelf()
        {
            FixAlphabetConfigs();
            FixPathConfigs();
        }

        private void FixAlphabetConfigs()
        {
            AlphabetConfigs ??= new();

            var toRemove = AlphabetConfigs.Where(x => x.Value?.Characters == null).Select(x => x.Key).ToList();
            foreach (var key in toRemove)
            {
                AlphabetConfigs.Remove(key);
            }
        }

        private void FixPathConfigs()
        {
            PathConfigs ??= new();

            var toRemove = PathConfigs.Where(x => x.Value?.Path == null).Select(x => x.Key).ToList();
            foreach (var key in toRemove)
            {
                PathConfigs.Remove(key);
            }

            foreach (var pathConfig in PathConfigs.Values)
            {
                pathConfig.SearchPattern ??= PathConfig.DefaultSearchPattern;
            }
        }

        protected override IEnumerable<Input> CollectChildren()
        {
            foreach (var input in base.CollectChildren())
            {
                yield return input;
            }

            foreach (var input in AlphabetConfigs.Values)
            {
                yield return input;
            }

            foreach (var input in PathConfigs.Values)
            {
                yield return input;
            }
        }
    }
}
