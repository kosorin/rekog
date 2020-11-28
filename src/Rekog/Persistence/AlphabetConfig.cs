using System.Collections.Generic;

namespace Rekog.Persistence
{
    public record AlphabetConfig : DataObject
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Characters { get; set; }

        public bool IncludeWhitespace { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void FixSelf()
        {
            Characters ??= string.Empty;
        }

        protected override IEnumerable<DataObject> CollectChildren()
        {
            yield break;
        }
    }
}
      