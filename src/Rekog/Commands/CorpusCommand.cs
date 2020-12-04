using Rekog.Controllers;
using Rekog.Persistence;
using Rekog.Persistence.Serialization;
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
            AddOption(new Option<bool>(new[] { "--extended", "-e" }));
        }

        protected override SerializerBase<CorpusCommandConfig> GetConfigSerializer()
        {
            return new CorpusCommandConfigSerializer();
        }
    }
}
