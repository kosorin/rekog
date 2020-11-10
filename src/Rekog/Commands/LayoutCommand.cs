using Rekog.Controllers;
using Rekog.Controllers.Options;
using System.CommandLine;
using System.IO;

namespace Rekog.Commands
{
    public class LayoutCommand : ControllerCommand<LayoutOptions, LayoutController>
    {
        public LayoutCommand() : base("layout")
        {
            AddArgument(new Argument<FileInfo>("layout"));
            AddArgument(new Argument<FileInfo>("ngrams"));
            AddOption(new Option<FileInfo?>(new[] { "--output", "-o" }));
            AddOption(new Option<bool>(new[] { "--same-character", "-s" }));
            AddOption(new Option<int?>(new[] { "--top-rank", "-r" }, () => 300));
            AddOption(new Option<double?>(new[] { "--threshold-percentage", "-p" }, () => 0.001));
        }
    }
}
