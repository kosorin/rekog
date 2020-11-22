using Rekog.Input.Configurations;
using Rekog.Input.Options;
using System.CommandLine;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public class CorpusController : CommandController<CorpusConfig, CorpusOptions>
    {
        public CorpusController(CorpusConfig config, IConsole console, IFileSystem fileSystem) : base(config, console, fileSystem)
        {
        }

        public override Task HandleAsync()
        {
            return Task.CompletedTask;
        }
    }
}
