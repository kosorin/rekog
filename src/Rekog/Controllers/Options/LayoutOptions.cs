using System.IO;

namespace Rekog.Controllers.Options
{
    public record LayoutOptions
    {
        public LayoutOptions(FileInfo layout, FileInfo ngrams)
        {
            Layout = layout;
            Ngrams = ngrams;
        }

        public FileInfo Layout { get; init; }

        public FileInfo Ngrams { get; init; }

        public FileInfo? Output { get; init; }

        public bool IncludeSameCharacter { get; init; }

        public int? TopRank { get; init; }

        public double? ThresholdPercentage { get; init; }
    }
}
