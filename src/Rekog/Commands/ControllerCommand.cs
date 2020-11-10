using Rekog.Controllers;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace Rekog.Commands
{
    public abstract class ControllerCommand<TOptions, TController> : Command
        where TOptions : class
        where TController : ControllerBase<TOptions>
    {
        public ControllerCommand(string name, string? description = null) : base(name, description)
        {
            Handler = CommandHandler.Create<TOptions, InvocationContext>(HandleAsync);
        }

        private Task HandleAsync(TOptions options, InvocationContext context)
        {
            if (Activator.CreateInstance(typeof(TController), options, context) is TController controller)
            {
                return controller.HandleAsync();
            }
            return Task.CompletedTask;
        }
    }
}
