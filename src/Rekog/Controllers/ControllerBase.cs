using Rekog.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Rekog.Controllers
{
    public abstract class ControllerBase<TOptions>
        where TOptions : class
    {
        protected ControllerBase(TOptions options, InvocationContext context)
        {
            Options = options;
            Context = context;
        }

        public TOptions Options { get; }

        public InvocationContext Context { get; }

        public ICommand Command => Context.ParseResult.CommandResult.Command;

        public IConsole Console => Context.Console;

        public abstract Task HandleAsync();

        protected virtual IDataWriter CreateOutputWriter(FileInfo? fileInfo)
        {
            return fileInfo != null
                ? new FileWriter(fileInfo.FullName)
                : new ConsoleWriter(Console.Out);
        }
    }
}
