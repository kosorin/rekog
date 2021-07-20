using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rekog.Data
{
    public record CorpusAnalysisReport : SerializationObject
    {
        public Dictionary<ReportToken, ulong> Unigrams { get; set; } = default!;

        public Dictionary<ReportToken, ulong> Bigrams { get; set; } = default!;

        public Dictionary<ReportToken, ulong> Trigrams { get; set; } = default!;

        public Dictionary<ReportToken, ulong> Replaced { get; set; } = default!;

        public List<SkippedToken> Skipped { get; set; } = default!;

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        protected override void FixSelf()
        {
            Unigrams ??= new Dictionary<ReportToken, ulong>();
            Bigrams ??= new Dictionary<ReportToken, ulong>();
            Trigrams ??= new Dictionary<ReportToken, ulong>();
            Replaced ??= new Dictionary<ReportToken, ulong>();
            Skipped ??= new List<SkippedToken>();
        }

        protected override IEnumerable<SerializationObject> CollectChildren()
        {
            foreach (var token in Unigrams.Keys)
            {
                yield return token;
            }
            foreach (var token in Bigrams.Keys)
            {
                yield return token;
            }
            foreach (var token in Trigrams.Keys)
            {
                yield return token;
            }
            foreach (var token in Replaced.Keys)
            {
                yield return token;
            }
            foreach (var token in Skipped)
            {
                yield return token;
            }
        }
    }
}
