using YamlDotNet.Serialization;

namespace Rekog.Data.Serialization
{
    public class CorpusReportSerializer : SerializerBase<CorpusAnalysisReport>
    {
        protected override void ConfigureSerializerBuilder(SerializerBuilder builder)
        {
            base.ConfigureSerializerBuilder(builder);

            builder.ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull);
        }
    }
}
