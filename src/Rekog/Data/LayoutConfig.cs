using System.Collections.Generic;

namespace Rekog.Data
{
    public record LayoutConfig : SerializationObject
    {
        public Matrix<int?> Fingers { get; set; } = default!;

        public Matrix<bool> Homing { get; set; } = default!;

        public Matrix<double> Efforts { get; set; } = default!;

        protected override void FixSelf()
        {
            Fingers ??= new();
            Fingers.Fix();

            Homing ??= new();
            Homing.Fix();

            Efforts ??= new();
            Efforts.Fix();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
