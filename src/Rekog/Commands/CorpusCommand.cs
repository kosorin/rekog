using Rekog.Controllers;
using Rekog.Input.Configurations;
using Rekog.Input.Options;
using Rekog.Serialization;
using System.CommandLine;
using System.IO.Abstractions;

namespace Rekog.Commands
{
    public class CorpusCommand : ControllerCommand<CorpusConfig, CorpusOptions, CorpusController>
    {
        public CorpusCommand(IFileSystem fileSystem) : base(fileSystem, "corpus")
        {
            AddOption(new Option<bool>(new[] { "--case-sensitive", "-s" }));
            AddOption(new Option<string[]>(new[] { "--alphabet", "-a" }));
            AddOption(new Option<string[]>(new[] { "--corpus", "-c" }));
            AddOption(new Option<bool>(new[] { "--include-ignored", "-i" }));
        }

        protected override ConfigDeserializer<CorpusConfig> GetConfigDeserializer()
        {
            return new CorpusConfigDeserializer();
        }
    }
}
