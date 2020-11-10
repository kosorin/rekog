using Rekog.Commands;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace Rekog
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args) => await BuildCommandLine()
            .UseDefaults()
            .Build()
            .InvokeAsync(args);

        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new RootCommand
            {
                new LayoutCommand(),
                new NgramCommand(),
            };
            return new CommandLineBuilder(root);
        }
    }
}
