using Rekog.Persistence;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Rekog.Persistence.Serialization
{
    public sealed class AlphabetConfigScalarNodeDeserializer : INodeDeserializer
    {
        bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
        {
            if (expectedType != typeof(AlphabetConfig) || !parser.TryConsume<Scalar>(out var scalar))
            {
                value = null;
                return false;
            }

            value = new AlphabetConfig
            {
                Characters = scalar.Value
            };
            return true;
        }
    }
}
