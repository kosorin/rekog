using YamlDotNet.Serialization;

namespace Rekog.Persistence.Serialization
{
    public class CorpusReportSerializer : SerializerBase<CorpusReport>
    {
        protected override void ConfigureSerializerBuilder(SerializerBuilder builder)
        {
            base.ConfigureSerializerBuilder(builder);

            builder.ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull);
        }
    }
}
