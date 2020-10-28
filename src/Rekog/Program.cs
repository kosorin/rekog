using Rekog.Commands.Ngram;
using System.CommandLine;
using System.Threading.Tasks;

namespace Rekog
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                NgramCommand.Create(),
            };
            return await rootCommand.InvokeAsync(args);
        }
    }
}
