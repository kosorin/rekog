using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Rekog.Data.Serialization
{
    public abstract class SerializerBase<TObject>
        where TObject : SerializationObject
    {
        private ISerializer? _serializer;
        private IDeserializer? _deserializer;

        protected INamingConvention DefaultNamingConvention { get; } = UnderscoredNamingConvention.Instance;

        public void Serialize(TextWriter writer, TObject obj)
        {
            var serializer = GetSerializer();
            serializer.Serialize(writer, obj);
            writer.Flush();
        }

        public TObject Deserialize(TextReader reader)
        {
            var deserializer = GetDeserializer();
            var obj = deserializer.Deserialize<TObject>(reader);
            obj.FixAll();
            return obj;
        }

        protected virtual void ConfigureBuilder<TBuilder>(BuilderSkeleton<TBuilder> builder)
            where TBuilder : BuilderSkeleton<TBuilder>
        {
            builder.WithNamingConvention(DefaultNamingConvention);
        }

        protected virtual void ConfigureSerializerBuilder(SerializerBuilder builder)
        {
        }

        protected virtual void ConfigureDeserializerBuilder(DeserializerBuilder builder)
        {
        }

        private ISerializer GetSerializer()
        {
            if (_serializer == null)
            {
                var builder = new SerializerBuilder();

                ConfigureBuilder(builder);
                ConfigureSerializerBuilder(builder);

                _serializer = builder.Build();
            }
            return _serializer;
        }

        private IDeserializer GetDeserializer()
        {
            if (_deserializer == null)
            {
                var builder = new DeserializerBuilder();

                ConfigureBuilder(builder);
                ConfigureDeserializerBuilder(builder);

                _deserializer = builder.Build();
            }
            return _deserializer;
        }
    }
}
