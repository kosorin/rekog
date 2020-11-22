using Rekog.Input.Configurations;
using YamlDotNet.Serialization;

namespace Rekog.Serialization
{
    public class CorpusConfigDeserializer : ConfigDeserializer<CorpusConfig>
    {
        protected override void ConfigureBuilder(DeserializerBuilder builder)
        {
            base.ConfigureBuilder(builder);

            builder
                .WithNodeDeserializer(new AlphabetConfigScalarNodeDeserializer())
                .WithNodeDeserializer(new PathConfigScalarNodeDeserializer())
                .WithAttributeOverride<CorpusConfig>(x => x.AlphabetConfigs, new YamlMemberAttribute { Alias = "alphabets" })
                .WithAttributeOverride<CorpusConfig>(x => x.PathConfigs, new YamlMemberAttribute { Alias = "corpora" })
                .IgnoreUnmatchedProperties();
        }
    }
}
