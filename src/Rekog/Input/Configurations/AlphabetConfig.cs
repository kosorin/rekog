using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rekog.Input.Configurations
{
    public class AlphabetConfig : Input
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Characters { get; set; }

        public bool IncludeWhitespace { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string GetCharacters()
        {
            var characters = Characters;

            if (!IncludeWhitespace)
            {
                characters = Regex.Replace(characters, @"\s+", "");
            }

            return characters;
        }

        protected override void FixSelf()
        {
            Characters ??= string.Empty;
        }

        protected override IEnumerable<Input> CollectChildren()
        {
            yield break;
        }
    }
}
