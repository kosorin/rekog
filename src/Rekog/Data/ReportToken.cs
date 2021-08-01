using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Rekog.Data
{
    public record ReportToken : SerializationObject, IYamlConvertible
    {
        public ReportToken()
        {
            // Ctor for deserialization
        }

        public ReportToken(string value)
        {
            Value = value;
        }

        public string Value { get; private set; } = null!;

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        protected override void FixSelf()
        {
            Value ??= string.Empty;
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            yield break;
        }

        void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            Value = (string?)nestedObjectDeserializer.Invoke(typeof(string)) ?? throw new InvalidOperationException();
        }

        void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            if (Value.EnumerateRunes().All(Rune.IsLetterOrDigit))
            {
                emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, Value, ScalarStyle.Plain, true, false));
            }
            else
            {
                var quoteStyle = Value.EnumerateRunes().Any(Rune.IsWhiteSpace) ? ScalarStyle.DoubleQuoted : ScalarStyle.SingleQuoted;
                emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, Value, quoteStyle, false, true));
            }
        }
    }
}
