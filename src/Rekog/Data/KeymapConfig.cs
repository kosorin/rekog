using System.Collections.Generic;
using System.Linq;

namespace Rekog.Data
{
    public record KeymapConfig : SerializationObject
    {
        public string[] Layers { get; set; } = default!;

        protected override void FixSelf()
        {
            Layers = Layers?.Where(x => x != null).ToArray() ?? new string[0];
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
