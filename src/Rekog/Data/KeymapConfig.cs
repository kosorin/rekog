using System;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.Data
{
    public record KeymapConfig : SerializationObject
    {
        public LayerConfig[] Layers { get; set; } = default!;

        protected override void FixSelf()
        {
            Layers = Layers?.Where(x => x != null).ToArray() ?? Array.Empty<LayerConfig>();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            return Layers;
        }
    }
}
