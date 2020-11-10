using System.IO;

namespace Rekog.Controllers.Options
{
    public record NgramOptions
    {
        public NgramOptions(string input)
        {
            Input = input;
        }

        public string Input { get; init; }

        public string? Pattern { get; init; }

        public FileInfo? Output { get; init; }

        public bool Raw { get; init; }

        public bool Analyzed { get; init; }

        public int Size { get; init; }

        public bool CaseSensitive { get; init; }

        public FileInfo[]? Alphabets { get; init; }

        public int? TopCount { get; init; }
    }
}
