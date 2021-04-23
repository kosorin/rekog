using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Data
{
    public record LayoutConfig : SerializationObject
    {
        public Matrix<int?> Fingers { get; set; } = default!;

        public Matrix<bool> Homing { get; set; } = default!;

        public Matrix<double> Efforts { get; set; } = default!;

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        protected override void FixSelf()
        {
            Fingers ??= new Matrix<int?>();
            Fingers.Fix();

            Homing ??= new Matrix<bool>();
            Homing.Fix();

            Efforts ??= new Matrix<double>();
            Efforts.Fix();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }
    }
}
