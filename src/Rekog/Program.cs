using Rekog.Commands;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;

namespace Rekog
{
    internal static class Program
    {
        private static Task<int> Main(string[] args)
        {
            return BuildCommandLine()
                .UseDefaults()
                .Build()
                .InvokeAsync(args);
        }

        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new RootCommand();

            foreach (var command in GetCommands())
            {
                root.AddCommand(command);
            }

            return new CommandLineBuilder(root);
        }

        private static IEnumerable<Command> GetCommands()
        {
            var baseType = typeof(ControllerCommand<,>);
            return baseType.Assembly.GetTypes()
                .Where(x => x.BaseType != null
                         && x.BaseType.IsGenericType
                         && x.BaseType.GetGenericTypeDefinition() == baseType)
                .Select(x => Activator.CreateInstance(x) is Command command ? command : throw new NullReferenceException());
        }
    }
}
