using Rekog.Persistence;
using System.CommandLine;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public class CorpusController : CommandController<CorpusCommandConfig, CorpusCommandOptions>
    {
        public CorpusController(CorpusCommandConfig config, IConsole console, IFileSystem fileSystem) : base(config, console, fileSystem)
        {
        }

        public override Task<int> HandleAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
