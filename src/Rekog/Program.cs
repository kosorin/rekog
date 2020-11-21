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
        // TODO: Handle errors/exceptions and return correct code (e.g. return 1 on error)
        private static Task<int> Main(string[] args)
        {
            return BuildParser().InvokeAsync(args);
        }

        private static Parser BuildParser()
        {
            return new CommandLineBuilder(GetRootCommand())
                .UseDefaults()
                .Build();
        }

        private static Command GetRootCommand()
        {
            var root = new RootCommand();
            foreach (var command in GetCommands())
            {
                root.AddCommand(command);
            }
            return root;
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
