using Rekog.Controllers;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rekog.Commands
{
    public abstract class ControllerCommand<TOptions, TController> : Command
        where TOptions : class
        where TController : ControllerBase<TOptions>
    {
        private static readonly FileSystem FileSystem;

        static ControllerCommand()
        {
            FileSystem = new FileSystem();
        }

        public ControllerCommand(string name, string? description = null) : base(name, description)
        {
            Handler = CommandHandler.Create<TOptions, IConsole>(HandleAsync);
        }

        private Task HandleAsync(TOptions options, IConsole console)
        {
            if (Activator.CreateInstance(typeof(TController), options, console, FileSystem) is TController controller)
            {
                return controller.HandleAsync();
            }
            return Task.CompletedTask;
        }
    }
}
