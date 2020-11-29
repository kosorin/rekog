using YamlDotNet.Serialization;

namespace Rekog.Persistence.Serialization
{
    public class CorpusCommandConfigSerializer : SerializerBase<CorpusCommandConfig>
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
