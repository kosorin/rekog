using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Data
{
    public record LayerConfig : SerializationObject
    {
        public Matrix<string?> Keys { get; set; } = default!;

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        protected override void FixSelf()
        {
            Keys ??= new Matrix<string?>();
            Keys.Fix();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
