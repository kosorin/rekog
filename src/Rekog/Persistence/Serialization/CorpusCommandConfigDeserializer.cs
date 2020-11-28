using Rekog.Persistence;
using YamlDotNet.Serialization;

namespace Rekog.Serialization
{
    public class CorpusCommandConfigDeserializer : DataObjectDeserializer<CorpusCommandConfig>
    {
        protected override void ConfigureBuilder(DeserializerBuilder builder)
        {
            base.ConfigureBuilder(builder);

            builder
                .WithNodeDeserializer(new AlphabetConfigScalarNodeDeserializer())
                .WithNodeDeserializer(new LocationConfigScalarNodeDeserializer())
                .WithAttributeOverride<CorpusCommandConfig>(x => x.Alphabets, new YamlMemberAttribute { Alias = "alphabets" })
                .WithAttributeOverride<CorpusCommandConfig>(x => x.Locations, new YamlMemberAttribute { Alias = "corpora" })
                .IgnoreUnmatchedProperties();
        }
    }
}
