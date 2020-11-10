using System.CommandLine;
using System.CommandLine.Invocation;
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
    }
}
