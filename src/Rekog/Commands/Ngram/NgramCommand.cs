using System.CommandLine;

namespace Rekog.Commands.Ngram
{
    public class NgramCommand : SubRootCommand
    {
        public static Command Create()
        {
            return new Command("ngram")
            {
                NgramAnalyzeCommand.Create(),
                NgramLayoutCommand.Create(),
            };
        }
    }
}
