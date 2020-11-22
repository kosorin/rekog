using Rekog.Input.Configurations;
using Rekog.Input.Options;
using Rekog.IO;
using System.CommandLine;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public abstract class CommandController<TConfig, TOptions>
        where TConfig : CommandConfig<TOptions>
        where TOptions : CommandOptions
    {
        protected CommandController(TConfig config, IConsole console, IFileSystem fileSystem)
        {
            Config = config;
            Console = console;
            FileSystem = fileSystem;
        }

        public TConfig Config { get; }

        public IConsole Console { get; }

        public IFileSystem FileSystem { get; }

        public abstract Task HandleAsync();

        protected virtual IDataReader CreateDataReader(string path)
        {
            return new DataReader(FileSystem.FileStream.Create(path, FileMode.Open));
        }

        protected virtual IDataWriter CreateDataWriter(string? path)
        {
            return path != null
                ? new DataWriter(FileSystem.FileStream.Create(path, FileMode.Create))
                : new ConsoleWriter(Console.Out);
        }
    }
}
