using Rekog.Controllers;
using Rekog.Controllers.Options;
using Rekog.IO;
using System.CommandLine;

namespace Rekog.Commands
{
    public class NgramCommand : ControllerCommand<NgramOptions, NgramController>
    {
        public NgramCommand() : base("ngram")
        {
            AddArgument(new Argument<string>("input", "Input file or directory path"));
            AddOption(new Option<string?>(new[] { "--pattern", "-p" }, () => PathHelper.DefaultSearchPattern, $"Search string to match against the names of files in path (used only if input is directory); If starts with '{PathHelper.SearchOptionPrefix}' then search in all subdirectories"));
            AddOption(new Option<string?>(new[] { "--output", "-o" }, "Output file path"));
            AddOption(new Option<bool>(new[] { "--raw" }));
            AddOption(new Option<bool>(new[] { "--analyzed" }));
            AddOption(new Option<int>(new[] { "--size", "-n" }, () => 2, "N-gram size"));
            AddOption(new Option<bool>(new[] { "--case-sensitive", "-c" }, "Case sensitive"));
            AddOption(new Option<string[]?>(new[] { "--alphabet", "-a" }) { Name = nameof(NgramOptions.Alphabets) });
            AddOption(new Option<int?>(new[] { "--top-count", "--top", "-t" }, "Number of n-grams to print to output (0 for all n-grams)"));
        }
    }
}
