using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Rekog.Serialization
{
    public abstract class ConfigDeserializer<TConfig>
        where TConfig : class
    {
        private IDeserializer? _deserializer;

        protected virtual INamingConvention DefaultNamingConvention { get; } = UnderscoredNamingConvention.Instance;

        public TConfig Deserialize(TextReader reader)
        {
            var deserializer = GetDeserializer();
            var config = deserializer.Deserialize<TConfig>(reader);

            return config;
        }

        protected virtual void ConfigureBuilder(DeserializerBuilder builder)
        {
            builder.WithNamingConvention(DefaultNamingConvention);
        }

        private IDeserializer GetDeserializer()
        {
            if (_deserializer == null)
            {
                var builder = new DeserializerBuilder();

                ConfigureBuilder(builder);

                _deserializer = builder.Build();
            }
            return _deserializer;
        }
    }
}
