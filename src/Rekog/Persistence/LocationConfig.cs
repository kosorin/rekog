using Rekog.IO;
using System.Collections.Generic;

namespace Rekog.Persistence
{
    public record LocationConfig : DataObject
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Path { get; set; }

        public string SearchPattern { get; set; }

        public bool Recursive { get; set; }

        public string Encoding { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void FixSelf()
        {
            Path ??= string.Empty;
            SearchPattern ??= PathHelper.DefaultSearchPattern;
            Encoding ??= System.Text.Encoding.UTF8.WebName;
        }

        protected override IEnumerable<DataObject> CollectChildren()
        {
            yield break;
        }
    }
}
