using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Rekog.Data
{
    public record SkippedToken : SerializationObject
    {
        public SkippedToken()
        {
            // Ctor for deserialization
        }

        public SkippedToken(ulong count, string value, int codePoint)
        {
            Value = new ReportToken(value);
            Count = count;
            Category = CharUnicodeInfo.GetUnicodeCategory(codePoint);
            Dec = codePoint.ToString();
            Hex = $"0x{codePoint:x4}";
        }

        public ReportToken Value { get; set; } = null!;

        public ulong Count { get; set; }

        public UnicodeCategory Category { get; set; }

        public string Dec { get; set; } = null!;

        public string Hex { get; set; } = null!;

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        protected override void FixSelf()
        {
            Value ??= new ReportToken();
            Dec ??= string.Empty;
            Hex ??= string.Empty;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield return Value;
        }
    }
}
