using Rekog.Controllers;
using Rekog.Persistence;
using Rekog.Serialization;
using System.CommandLine;
using System.IO.Abstractions;

namespace Rekog.Commands
{
    public class CorpusCommand : ControllerCommand<CorpusCommandConfig, CorpusCommandOptions, CorpusController>
    {
        public CorpusCommand(IFileSystem fileSystem) : base(fileSystem, "corpus")
        {
            AddOption(new Option<bool>(new[] { "--case-sensitive", "-s" }));
            AddOption(new Option<string[]>(new[] { "--alphabet", "-a" }));
            AddOption(new Option<string[]>(new[] { "--corpus", "-c" }));
            AddOption(new Option<bool>(new[] { "--include-ignored", "-i" }));
        }

        protected override DataObjectDeserializer<CorpusCommandConfig> GetConfigDeserializer()
        {
            return new CorpusCommandConfigDeserializer();
        }
    }
}
