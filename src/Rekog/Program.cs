using Rekog.Commands.Corpus;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace Rekog
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            return await BuildCommandLine()
                .UseDefaults()
                .Build()
                .InvokeAsync(@"corpus D:\");
        }

        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new RootCommand
            {
                new CorpusCommand(),
            };
            return new CommandLineBuilder(root);
        }
    }
}
