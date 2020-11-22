using Rekog.Input.Configurations;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Rekog.Serialization
{
    public sealed class PathConfigScalarNodeDeserializer : INodeDeserializer
    {
        bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
        {
            if (expectedType != typeof(PathConfig) || !parser.TryConsume<Scalar>(out var scalar))
            {
                value = null;
                return false;
            }

            value = new PathConfig
            {
                Path = scalar.Value
            };
            return true;
        }
    }
}
