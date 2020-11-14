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

        public string? Output { get; init; }

        public bool Raw { get; init; }

        public bool Analyzed { get; init; }

        public int Size { get; init; }

        public bool CaseSensitive { get; init; }

        public string[]? Alphabets { get; init; }

        public int? TopCount { get; init; }
    }
}
