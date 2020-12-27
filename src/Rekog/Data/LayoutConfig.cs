using System.Collections.Generic;

namespace Rekog.Data
{
    public record LayoutConfig : SerializationObject
    {
        public Matrix<int?> Fingers { get; set; } = default!;

        public Matrix<bool> Homing { get; set; } = default!;

        protected override void FixSelf()
        {
            Fingers ??= new();
            Fingers.Fix();

            Homing ??= new();
            Homing.Fix();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
