using Rekog.IO;
using System.CommandLine;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rekog.Commands
{
    public abstract class ControllerBase<TOptions>
        where TOptions : class
    {
        protected ControllerBase(TOptions options, IConsole console, IFileSystem fileSystem)
        {
            Options = options;
            Console = console;
            FileSystem = fileSystem;
        }

        public TOptions Options { get; }

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
