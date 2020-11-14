namespace Rekog.Controllers.Options
{
    public record LayoutOptions
    {
        public LayoutOptions(string layout, string ngrams)
        {
            Layout = layout;
            Ngrams = ngrams;
        }

        public string Layout { get; init; }

        public string Ngrams { get; init; }

        public string? Output { get; init; }

        public bool IncludeSameCharacter { get; init; }

        public int? TopRank { get; init; }

        public double? ThresholdPercentage { get; init; }
    }
}
