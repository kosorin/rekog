using System.Collections.Generic;

namespace Rekog.Data
{
    public record LayerConfig : SerializationObject
    {
        public Matrix<char?> Keys { get; set; } = default!;

        protected override void FixSelf()
        {
            Keys ??= new();
            Keys.Fix();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
