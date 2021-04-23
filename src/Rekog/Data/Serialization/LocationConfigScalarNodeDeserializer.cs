using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Rekog.Data.Serialization
{
    public sealed class LocationConfigScalarNodeDeserializer : INodeDeserializer
    {
        bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
        {
            if (expectedType != typeof(CorpusConfig) || !parser.TryConsume<Scalar>(out var scalar))
            {
                value = null;
                return false;
            }

            value = new CorpusConfig
            {
                Path = scalar.Value,
            };
            return true;
        }
    }
}
