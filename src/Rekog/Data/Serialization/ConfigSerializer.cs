using YamlDotNet.Serialization;

namespace Rekog.Data.Serialization
{
    public class ConfigSerializer : SerializerBase<Config>
    {
        protected override void ConfigureDeserializerBuilder(DeserializerBuilder builder)
        {
            base.ConfigureDeserializerBuilder(builder);

            builder
                .WithNodeDeserializer(new AlphabetConfigScalarNodeDeserializer())
                .WithNodeDeserializer(new LocationConfigScalarNodeDeserializer());
        }
    }
}
